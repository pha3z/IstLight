# Copyright 2012 Jakub Niemyjski
#
# This file is part of IstLight.
#
# IstLight is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# IstLight is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with IstLight.  If not, see <http://www.gnu.org/licenses/>.

import System
from System import ArgumentException
import clr
clr.AddReference("System.Core")
clr.AddReference("IstLight.Domain");
from IstLight.Services import *
from IstLight import *
from System.Collections.Generic import *
from System.Net import *
from System import DateTime,Double,String, Array, StringSplitOptions
from System.Globalization import NumberStyles,CultureInfo
from System.Text import Encoding
from System.Linq import Enumerable
clr.ImportExtensions(System.Linq)

providerSiteUrl = "http://ichart.yahoo.com"
def ProviderGetTickerDataUrl(ticker): return "http://ichart.yahoo.com/table.csv?s="+ticker
def ProviderGetTickersUrl(ticker): return "http://d.yimg.com/aq/autoc?query="+ticker+"&callback=YAHOO.util.ScriptNodeDataSource.callbacks"
Name = "Yahoo.com"

class WebClientEx(WebClient):
	def GetWebRequest(self,address):
		request = WebClient.GetWebRequest(self,address)
		request.CookieContainer = self.CookieContainer
		request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
		return request

def GetCookieContainer(url):
	cookieContainer = CookieContainer()
	request = WebRequest.Create(url)
	request.CookieContainer = cookieContainer
	request.Method = "HEAD"
	response = request.GetResponse()
	response.Close()
	return cookieContainer
	
def GetRawData(webUrl, fileUrl):
	result = None
	with WebClientEx() as client:
		client.CookieContainer = GetCookieContainer(webUrl)
		client.Encoding = Encoding.UTF8
		result = client.DownloadString(fileUrl)
	if (result is None) or (result == ''):
		raise Exception('Can\'t download data')
	elif result.Length < 10:
		raise Exception('No data')
	return result

def GetIndexesOf(input,c):
	return input.Select(lambda x,i: (x,i)).Where(lambda x: x[0] == c).Select(lambda x: x[1])

def AsPairs(collection):
	return collection.Where(lambda x, i: i % 2 == 0).Zip(collection.Where(lambda y, j: (j + 1) % 2 == 0), lambda x1, x2: (x1,x2))

def ExtractSearchResultFromLine(line):
	items = AsPairs(GetIndexesOf(line, '"')).Select(lambda x: line[x[0]+1:][:x[1]-x[0]-1])
	pairs = {}
	for p in AsPairs(items): pairs[p[0]] = p[1]
	symbol = pairs["symbol"]
	desc = pairs["name"]
	market = pairs.get("exchDisp", pairs.get("exch", String.Empty))
	return TickerSearchResult(symbol,desc,market)

def ExtractDate(x): return DateTime.Parse(x,CultureInfo.InvariantCulture)
def ExtractDouble(x): return Double.Parse(x, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
def ExtractIndices(header):
	indices = {}
	indices['date'] = header.IndexOf('Date')
	indices['open'] = header.IndexOf('Open')
	indices['high'] = header.IndexOf('High')
	indices['low'] = header.IndexOf('Low')
	indices['close'] = header.IndexOf('Close')
	indices['volume'] = header.IndexOf('Volume')
	indices['adjClose'] = header.IndexOf('Adj Close')
	return indices
	
def ExtractQuote(row, indices):
	date = ExtractDate(row[indices['date']])
	open = ExtractDouble(row[indices['open']])
	high = ExtractDouble(row[indices['high']])
	low = ExtractDouble(row[indices['low']])
	close = ExtractDouble(row[indices['close']])
	if indices['volume'] >= 0: volume = ExtractDouble(row[indices['volume']])
	else: volume = None
	if indices['adjClose'] >= 0:
		ratio = ExtractDouble(row[indices['adjClose']]) / close
		open *= ratio
		high *= ratio
		low *= ratio
		close *= ratio
	return TickerQuote(date,open,close,high,low,volume)

#Download ticker
def Get(ticker):
	rawData = GetRawData(providerSiteUrl, ProviderGetTickerDataUrl(ticker))
	rawRows = rawData.Replace(oldValue = '\r', newValue = '').Split('\n')
	indices = ExtractIndices(rawRows.First().Split(',').ToList())
	rawRows = rawRows.Skip(1).TakeWhile(lambda row: row.Length >= 5).Select(lambda row: row.Split(','))
	quotes = Array[TickerQuote](rawRows.Select(lambda row: ExtractQuote(row, indices)).ToArray())
	quotes = IReadOnlyListExtensions.AsReadOnlyList(quotes)
	return Ticker(ticker,quotes)

#Find available tickers
def Search(hint):
	if(String.IsNullOrWhiteSpace(hint)): return Array[TickerSearchResult]([])
	input = GetRawData(providerSiteUrl, ProviderGetTickersUrl(hint))
	if input == None: return Array[TickerSearchResult]([])
	start = input.IndexOf('[');
	end = input.IndexOf(']');
	if (start < 0 or end < 0): return Array[TickerSearchResult]([])
	input = input[start+1:][:end-start-1] #input.Substring(start+1, end - start - 1);
	input = input.Split(Array[str](["},{"]), StringSplitOptions.RemoveEmptyEntries).Select(lambda x: x.Replace("{", "")).Select(lambda x: x.Replace("}", ""))
	return Array[TickerSearchResult](input.Select(lambda x: ExtractSearchResultFromLine(x)).ToArray())