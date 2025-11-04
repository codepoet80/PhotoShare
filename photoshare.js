window.queryString = new Object();
findQueryParams();

function findQueryParams(url)
{
	if (url == null)
		url = document.location + "";
	var urlParts = url.split("?")
	if (urlParts.length > 1)
	{
		urlParts = urlParts[1].split("&");
		for (var q=0;q<urlParts.length;q++)
		{
			var paramParts = urlParts[q].split("=");
			if (paramParts.length > 1)
			{
				eval ("window.queryString." + paramParts[0] + "=\"" + paramParts[1] + "\"");
			}
		}
	}
}

function replace(oldstring, match, newchar)
{
	var FC = oldstring.substr(0, 1);
	var LC = oldstring.substr(oldstring.length-1, 1);
	var oldParts = oldstring.split(match);
	var newString = "";
	for (var j=0;j<oldParts.length;j++)
	{
		if (oldParts[j-1] == null)
		{
			newString += oldParts[j];
		}
		else
		{
			newString += newchar + oldParts[j];
		}
	}
	return newString;
}

function urlEncode(oldstring)
{
	var encodeVal;
	encodeVal = oldstring;
	//encodeVal = replace(encodeVal, "%", "%25");
	encodeVal = replace(encodeVal, "&", "%26");
	encodeVal = replace(encodeVal, " ", "%20");
	encodeVal = replace(encodeVal, "#", "%23");
	encodeVal = replace(encodeVal, "+", "%2B");
	encodeVal = replace(encodeVal, "'", "%27");
	encodeVal = replace(encodeVal, ",", "%2C");
	return encodeVal;
}

function urlDecode(oldstring)
{
	var decodeVal;
	decodeVal = oldstring;
	decodeVal = replace(decodeVal, "%20", " ");
	decodeVal = replace(decodeVal, "%23", "#");
	decodeVal = replace(decodeVal, "%2B", "+");
	decodeVal = replace(decodeVal, "%27", "'");
	decodeVal = replace(decodeVal, "%26", "&");
	decodeVal = replace(decodeVal, "%2C", ",");
	//decodeVal = replace(decodeVal, "%25", "%");
	return decodeVal;
}

function BuildSOAPQuery(soapProps)
{
	queryString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
	queryString+= "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
	queryString+= "<soap:Body>";
	queryString+= "<" + soapProps.Function + " xmlns=\"" + soapProps.Namespace + "\">";
	for (var i in soapProps)
	{
		if (typeof(soapProps[i])!="object")			
		{
			if (i != "Function" && i != "Namespace")
			{
				queryString += "<" + i + ">";
				queryString += eval("soapProps." + i);
				queryString += "</" + i + ">";
			}
		}
	}
	
	queryString+= "</" + soapProps.Function + ">"
	queryString+= "</soap:Body>";
	queryString+= "</soap:Envelope>";
	return queryString;
}

function SendSOAPCall(src, host, contentType, fnamespace, fname, queryString)
{
	var contentType = "text/xml";;
	if (browser == "IE")
	{
		var xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
		xmlhttp.Open("POST",src, false);
		xmlhttp.setRequestHeader("Host", host);
		xmlhttp.setRequestHeader("Content-type", contentType);
		xmlhttp.setRequestHeader("SOAPAction", fnamespace + "/" + fname);
		xmlhttp.send(queryString);
		var response = xmlhttp.responseXML.xml;
	}
	else
	{
		var xmlhttp = new XMLHttpRequest();
		xmlhttp.open ("Post", src, false);
		xmlhttp.setRequestHeader("Host", host);
		xmlhttp.setRequestHeader("Content-type", contentType);
		xmlhttp.setRequestHeader("SOAPAction", fnamespace + "/" + fname);
		xmlhttp.send(queryString);
		var response = xmlhttp.responseText;
	}
	return response;
}

function getFiles(folder, filter)
{
	var soapProps = new Object();
	soapProps.Function = "ListFiles";
	soapProps.Namespace = "https://github.com/codepoet80/photoshare";
	soapProps.Folder = folder;
	soapProps.Filter = filter;
	
	var queryString = BuildSOAPQuery(soapProps);
	response = SendSOAPCall("photoshare.asmx", document.location.host, null, soapProps.Namespace, soapProps.Function, queryString)
	return(response);
}
