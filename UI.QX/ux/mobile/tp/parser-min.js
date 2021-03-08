var parser=function(t){var e={};function r(a){if(e[a])return e[a].exports;var i=e[a]={i:a,l:!1,exports:{}};return t[a].call(i.exports,i,i.exports,r),i.l=!0,i.exports}return r.m=t,r.c=e,r.d=function(t,e,a){r.o(t,e)||Object.defineProperty(t,e,{enumerable:!0,get:a})},r.r=function(t){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(t,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(t,"__esModule",{value:!0})},r.t=function(t,e){if(1&e&&(t=r(t)),8&e)return t;if(4&e&&"object"==typeof t&&t&&t.__esModule)return t;var a=Object.create(null);if(r.r(a),Object.defineProperty(a,"default",{enumerable:!0,value:t}),2&e&&"string"!=typeof t)for(var i in t)r.d(a,i,function(e){return t[e]}.bind(null,i));return a},r.n=function(t){var e=t&&t.__esModule?function(){return t.default}:function(){return t};return r.d(e,"a",e),e},r.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},r.p="",r(r.s=2)}([function(t,e,r){"use strict";var a=":A-Za-z_\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u02FF\\u0370-\\u037D\\u037F-\\u1FFF\\u200C-\\u200D\\u2070-\\u218F\\u2C00-\\u2FEF\\u3001-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFFD",i="["+a+"][:A-Za-z_\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u02FF\\u0370-\\u037D\\u037F-\\u1FFF\\u200C-\\u200D\\u2070-\\u218F\\u2C00-\\u2FEF\\u3001-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFFD\\-.\\d\\u00B7\\u0300-\\u036F\\u203F-\\u2040]*",n=new RegExp("^"+i+"$");e.isExist=function(t){return void 0!==t},e.isEmptyObject=function(t){return 0===Object.keys(t).length},e.merge=function(t,e,r){if(e)for(var a=Object.keys(e),i=a.length,n=0;n<i;n++)t[a[n]]="strict"===r?[e[a[n]]]:e[a[n]]},e.getValue=function(t){return e.isExist(t)?t:""},e.buildOptions=function(t,e,r){var a={};if(!t)return e;for(var i=0;i<r.length;i++)void 0!==t[r[i]]?a[r[i]]=t[r[i]]:a[r[i]]=e[r[i]];return a},e.isTagNameInArrayMode=function(t,e,r){return!1!==e&&(e instanceof RegExp?e.test(t):"function"==typeof e?!!e(t,r):"strict"===e)},e.isName=function(t){var e=n.exec(t);return!(null==e)},e.getAllMatches=function(t,e){for(var r=[],a=e.exec(t);a;){for(var i=[],n=a.length,s=0;s<n;s++)i.push(a[s]);r.push(i),a=e.exec(t)}return r},e.nameRegexp=i},function(t,e,r){"use strict";var a=r(0),i=r(0).buildOptions,n=r(4);"<((!\\[CDATA\\[([\\s\\S]*?)(]]>))|((NAME:)?(NAME))([^>]*)>|((\\/)(NAME)\\s*>))([^<]*)".replace(/NAME/g,a.nameRegexp);!Number.parseInt&&window.parseInt&&(Number.parseInt=window.parseInt),!Number.parseFloat&&window.parseFloat&&(Number.parseFloat=window.parseFloat);var s={attributeNamePrefix:"@_",attrNodeName:!1,textNodeName:"#text",ignoreAttributes:!0,ignoreNameSpace:!1,allowBooleanAttributes:!1,parseNodeValue:!0,parseAttributeValue:!1,arrayMode:!1,trimValues:!0,cdataTagName:!1,cdataPositionChar:"\\c",tagValueProcessor:function(t,e){return t},attrValueProcessor:function(t,e){return t},stopNodes:[]};e.defaultOptions=s;var o=["attributeNamePrefix","attrNodeName","textNodeName","ignoreAttributes","ignoreNameSpace","allowBooleanAttributes","parseNodeValue","parseAttributeValue","arrayMode","trimValues","cdataTagName","cdataPositionChar","tagValueProcessor","attrValueProcessor","parseTrueNumberOnly","stopNodes"];function u(t,e,r){return e&&(r.trimValues&&(e=e.trim()),e=d(e=r.tagValueProcessor(e,t),r.parseNodeValue,r.parseTrueNumberOnly)),e}function l(t,e){if(e.ignoreNameSpace){var r=t.split(":"),a="/"===t.charAt(0)?"/":"";if("xmlns"===r[0])return"";2===r.length&&(t=a+r[1])}return t}function d(t,e,r){var i;return e&&"string"==typeof t?(""===t.trim()||isNaN(t)?i="true"===t||"false"!==t&&t:(-1!==t.indexOf("0x")?i=Number.parseInt(t,16):-1!==t.indexOf(".")?(i=Number.parseFloat(t),t=t.replace(/\.?0+$/,"")):i=Number.parseInt(t,10),r&&(i=String(i)===t?i:t)),i):a.isExist(t)?t:""}e.props=o;var f=new RegExp("([^\\s=]+)\\s*(=\\s*(['\"])(.*?)\\3)?","g");function c(t,e){if(!e.ignoreAttributes&&"string"==typeof t){t=t.replace(/\r?\n/g," ");for(var r=a.getAllMatches(t,f),i=r.length,n={},s=0;s<i;s++){var o=l(r[s][1],e);o.length&&(void 0!==r[s][4]?(e.trimValues&&(r[s][4]=r[s][4].trim()),r[s][4]=e.attrValueProcessor(r[s][4],o),n[e.attributeNamePrefix+o]=d(r[s][4],e.parseAttributeValue,e.parseTrueNumberOnly)):e.allowBooleanAttributes&&(n[e.attributeNamePrefix+o]=!0))}if(!Object.keys(n).length)return;if(e.attrNodeName){var u={};return u[e.attrNodeName]=n,u}return n}}function h(t,e){for(var r,a="",i=e;i<t.length;i++){var n=t[i];if(r)n===r&&(r="");else if('"'===n||"'"===n)r=n;else{if(">"===n)return{data:a,index:i};"\t"===n&&(n=" ")}a+=n}}function g(t,e,r,a){var i=t.indexOf(e,r);if(-1===i)throw new Error(a);return i+e.length-1}e.getTraversalObj=function(t,e){t=t.replace(/\r\n?/g,"\n"),e=i(e,s,o);for(var r=new n("!xml"),l=r,d="",f=0;f<t.length;f++){if("<"===t[f])if("/"===t[f+1]){var v=g(t,">",f,"Closing Tag is not closed."),p=t.substring(f+2,v).trim();if(e.ignoreNameSpace){var m=p.indexOf(":");-1!==m&&(p=p.substr(m+1))}l&&(l.val?l.val=a.getValue(l.val)+""+u(p,d,e):l.val=u(p,d,e)),e.stopNodes.length&&e.stopNodes.includes(l.tagname)&&(l.child=[],null==l.attrsMap&&(l.attrsMap={}),l.val=t.substr(l.startIndex+1,f-l.startIndex-1)),l=l.parent,d="",f=v}else if("?"===t[f+1])f=g(t,"?>",f,"Pi Tag is not closed.");else if("!--"===t.substr(f+1,3))f=g(t,"--\x3e",f,"Comment is not closed.");else if("!D"===t.substr(f+1,2)){var b=g(t,">",f,"DOCTYPE is not closed.");f=t.substring(f,b).indexOf("[")>=0?t.indexOf("]>",f)+1:b}else if("!["===t.substr(f+1,2)){var N=g(t,"]]>",f,"CDATA is not closed.")-2,x=t.substring(f+9,N);if(d&&(l.val=a.getValue(l.val)+""+u(l.tagname,d,e),d=""),e.cdataTagName){var A=new n(e.cdataTagName,l,x);l.addChild(A),l.val=a.getValue(l.val)+e.cdataPositionChar,x&&(A.val=x)}else l.val=(l.val||"")+(x||"");f=N+2}else{var y=h(t,f+1),C=y.data,T=y.index,O=C.indexOf(" "),P=C;if(-1!==O&&(P=C.substr(0,O).replace(/\s\s*$/,""),C=C.substr(O+1)),e.ignoreNameSpace){var F=P.indexOf(":");-1!==F&&(P=P.substr(F+1))}if(l&&d&&"!xml"!==l.tagname&&(l.val=a.getValue(l.val)+""+u(l.tagname,d,e)),C.length>0&&C.lastIndexOf("/")===C.length-1){C="/"===P[P.length-1]?P=P.substr(0,P.length-1):C.substr(0,C.length-1);var j=new n(P,l,"");P!==C&&(j.attrsMap=c(C,e)),l.addChild(j)}else{var E=new n(P,l);e.stopNodes.length&&e.stopNodes.includes(E.tagname)&&(E.startIndex=T),P!==C&&(E.attrsMap=c(C,e)),l.addChild(E),l=E}d="",f=T}else d+=t[f]}return r}},function(t,e,r){"use strict";var a=r(3),i=r(1),n=r(1),s=r(0).buildOptions,o=r(5);e.parse=function(t,e,r){if(r){!0===r&&(r={});var u=o.validate(t,r);if(!0!==u)throw Error(u.err.msg)}e=s(e,n.defaultOptions,n.props);var l=i.getTraversalObj(t,e);return a.convertToJson(l,e)},e.convertTonimn=r(6).convert2nimn,e.getTraversalObj=i.getTraversalObj,e.convertToJson=a.convertToJson,e.convertToJsonString=r(7).convertToJsonString,e.validate=o.validate,e.j2xParser=r(8),e.parseToNimn=function(t,r,a){return e.convertTonimn(e.getTraversalObj(t,a),r,a)}},function(t,e,r){"use strict";var a=r(0);e.convertToJson=function t(e,r,i){var n={};if((!e.child||a.isEmptyObject(e.child))&&(!e.attrsMap||a.isEmptyObject(e.attrsMap)))return a.isExist(e.val)?e.val:"";if(a.isExist(e.val)&&("string"!=typeof e.val||""!==e.val&&e.val!==r.cdataPositionChar)){var s=a.isTagNameInArrayMode(e.tagname,r.arrayMode,i);n[r.textNodeName]=s?[e.val]:e.val}a.merge(n,e.attrsMap,r.arrayMode);for(var o=Object.keys(e.child),u=0;u<o.length;u++){var l=o[u];if(e.child[l]&&e.child[l].length>1)for(var d in n[l]=[],e.child[l])e.child[l].hasOwnProperty(d)&&n[l].push(t(e.child[l][d],r,l));else{var f=t(e.child[l][0],r,l),c=!0===r.arrayMode&&"object"==typeof f||a.isTagNameInArrayMode(l,r.arrayMode,i);n[l]=c?[f]:f}}return n}},function(t,e,r){"use strict";t.exports=function(t,e,r){this.tagname=t,this.parent=e,this.child={},this.attrsMap={},this.val=r,this.addChild=function(t){Array.isArray(this.child[t.tagname])?this.child[t.tagname].push(t):this.child[t.tagname]=[t]}}},function(t,e,r){"use strict";var a=r(0),i={allowBooleanAttributes:!1},n=["allowBooleanAttributes"];function s(t,e){for(var r=e;e<t.length;e++)if("?"!=t[e]&&" "!=t[e]);else{var a=t.substr(r,e-r);if(e>5&&"xml"===a)return c("InvalidXml","XML declaration allowed only at the start of the document.",g(t,e));if("?"==t[e]&&">"==t[e+1]){e++;break}}return e}function o(t,e){if(t.length>e+5&&"-"===t[e+1]&&"-"===t[e+2]){for(e+=3;e<t.length;e++)if("-"===t[e]&&"-"===t[e+1]&&">"===t[e+2]){e+=2;break}}else if(t.length>e+8&&"D"===t[e+1]&&"O"===t[e+2]&&"C"===t[e+3]&&"T"===t[e+4]&&"Y"===t[e+5]&&"P"===t[e+6]&&"E"===t[e+7]){var r=1;for(e+=8;e<t.length;e++)if("<"===t[e])r++;else if(">"===t[e]&&0===--r)break}else if(t.length>e+9&&"["===t[e+1]&&"C"===t[e+2]&&"D"===t[e+3]&&"A"===t[e+4]&&"T"===t[e+5]&&"A"===t[e+6]&&"["===t[e+7])for(e+=8;e<t.length;e++)if("]"===t[e]&&"]"===t[e+1]&&">"===t[e+2]){e+=2;break}return e}e.validate=function(t,e){e=a.buildOptions(e,i,n);var r,l=[],h=!1,v=!1;"\ufeff"===t[0]&&(t=t.substr(1));for(var p=0;p<t.length;p++)if("<"===t[p]&&"?"===t[p+1]){if((p=s(t,p+=2)).err)return p}else{if("<"!==t[p]){if(" "===t[p]||"\t"===t[p]||"\n"===t[p]||"\r"===t[p])continue;return c("InvalidChar","char '"+t[p]+"' is not expected.",g(t,p))}if("!"===t[++p]){p=o(t,p);continue}var m=!1;"/"===t[p]&&(m=!0,p++);for(var b="";p<t.length&&">"!==t[p]&&" "!==t[p]&&"\t"!==t[p]&&"\n"!==t[p]&&"\r"!==t[p];p++)b+=t[p];if("/"===(b=b.trim())[b.length-1]&&(b=b.substring(0,b.length-1),p--),r=b,!a.isName(r)){return c("InvalidTag",0===b.trim().length?"There is an unnecessary space between tag name and backward slash '</ ..'.":"Tag '"+b+"' is an invalid name.",g(t,p))}var N=u(t,p);if(!1===N)return c("InvalidAttr","Attributes for '"+b+"' have open quote.",g(t,p));var x=N.value;if(p=N.index,"/"===x[x.length-1]){var A=d(x=x.substring(0,x.length-1),e);if(!0!==A)return c(A.err.code,A.err.msg,g(t,p-x.length+A.err.line));h=!0}else if(m){if(!N.tagClosed)return c("InvalidTag","Closing tag '"+b+"' doesn't have proper closing.",g(t,p));if(x.trim().length>0)return c("InvalidTag","Closing tag '"+b+"' can't have attributes or invalid starting.",g(t,p));var y=l.pop();if(b!==y)return c("InvalidTag","Closing tag '"+y+"' is expected inplace of '"+b+"'.",g(t,p));0==l.length&&(v=!0)}else{var C=d(x,e);if(!0!==C)return c(C.err.code,C.err.msg,g(t,p-x.length+C.err.line));if(!0===v)return c("InvalidXml","Multiple possible root nodes found.",g(t,p));l.push(b),h=!0}for(p++;p<t.length;p++)if("<"===t[p]){if("!"===t[p+1]){p=o(t,++p);continue}if("?"!==t[p+1])break;if((p=s(t,++p)).err)return p}else if("&"===t[p]){var T=f(t,p);if(-1==T)return c("InvalidChar","char '&' is not expected.",g(t,p));p=T}"<"===t[p]&&p--}return h?!(l.length>0)||c("InvalidXml","Invalid '"+JSON.stringify(l,null,4).replace(/\r?\n/g,"")+"' found.",1):c("InvalidXml","Start tag expected.",1)};function u(t,e){for(var r="",a="",i=!1;e<t.length;e++){if('"'===t[e]||"'"===t[e])if(""===a)a=t[e];else{if(a!==t[e])continue;a=""}else if(">"===t[e]&&""===a){i=!0;break}r+=t[e]}return""===a&&{value:r,index:e,tagClosed:i}}var l=new RegExp("(\\s*)([^\\s=]+)(\\s*=)?(\\s*(['\"])(([\\s\\S])*?)\\5)?","g");function d(t,e){for(var r=a.getAllMatches(t,l),i={},n=0;n<r.length;n++){if(0===r[n][1].length)return c("InvalidAttr","Attribute '"+r[n][2]+"' has no space in starting.",v(t,r[n][0]));if(void 0===r[n][3]&&!e.allowBooleanAttributes)return c("InvalidAttr","boolean attribute '"+r[n][2]+"' is not allowed.",v(t,r[n][0]));var s=r[n][2];if(!h(s))return c("InvalidAttr","Attribute '"+s+"' is an invalid name.",v(t,r[n][0]));if(i.hasOwnProperty(s))return c("InvalidAttr","Attribute '"+s+"' is repeated.",v(t,r[n][0]));i[s]=1}return!0}function f(t,e){if(";"===t[++e])return-1;if("#"===t[e])return function(t,e){var r=/\d/;for("x"===t[e]&&(e++,r=/[\da-fA-F]/);e<t.length;e++){if(";"===t[e])return e;if(!t[e].match(r))break}return-1}(t,++e);for(var r=0;e<t.length;e++,r++)if(!(t[e].match(/\w/)&&r<20)){if(";"===t[e])break;return-1}return e}function c(t,e,r){return{err:{code:t,msg:e,line:r}}}function h(t){return a.isName(t)}function g(t,e){return t.substring(0,e).split(/\r?\n/).length}function v(t,e){return t.indexOf(e)+e.length}},function(t,e,r){"use strict";var a=function(t){return String.fromCharCode(t)},i={nilChar:a(176),missingChar:a(201),nilPremitive:a(175),missingPremitive:a(200),emptyChar:a(178),emptyValue:a(177),boundryChar:a(179),objStart:a(198),arrStart:a(204),arrayEnd:a(185)},n=[i.nilChar,i.nilPremitive,i.missingChar,i.missingPremitive,i.boundryChar,i.emptyChar,i.emptyValue,i.arrayEnd,i.objStart,i.arrStart],s=function t(e,r,a){if("string"==typeof r)return e&&e[0]&&void 0!==e[0].val?o(e[0].val,r):o(e,r);var n,s=void 0===(n=e)?i.missingChar:null===n?i.nilChar:!(n.child&&0===Object.keys(n.child).length&&(!n.attrsMap||0===Object.keys(n.attrsMap).length))||i.emptyChar;if(!0===s){var l="";if(Array.isArray(r)){l+=i.arrStart;var d=r[0],f=e.length;if("string"==typeof d)for(var c=0;c<f;c++){var h=o(e[c].val,d);l=u(l,h)}else for(var g=0;g<f;g++){var v=t(e[g],d,a);l=u(l,v)}l+=i.arrayEnd}else{l+=i.objStart;var p=Object.keys(r);for(var m in Array.isArray(e)&&(e=e[0]),p){var b=p[m],N=void 0;N=!a.ignoreAttributes&&e.attrsMap&&e.attrsMap[b]?t(e.attrsMap[b],r[b],a):b===a.textNodeName?t(e.val,r[b],a):t(e.child[b],r[b],a),l=u(l,N)}}return l}return s},o=function(t){switch(t){case void 0:return i.missingPremitive;case null:return i.nilPremitive;case"":return i.emptyValue;default:return t}},u=function(t,e){return l(e[0])||l(t[t.length-1])||(t+=i.boundryChar),t+e},l=function(t){return-1!==n.indexOf(t)};var d=r(1),f=r(0).buildOptions;e.convert2nimn=function(t,e,r){return r=f(r,d.defaultOptions,d.props),s(t,e,r)}},function(t,e,r){"use strict";var a=r(0),i=r(0).buildOptions,n=r(1),s=function t(e,r,i){for(var n,s="{",o=Object.keys(e.child),u=0;u<o.length;u++){var l=o[u];if(e.child[l]&&e.child[l].length>1){for(var d in s+='"'+l+'" : [ ',e.child[l])s+=t(e.child[l][d],r)+" , ";s=s.substr(0,s.length-1)+" ] "}else s+='"'+l+'" : '+t(e.child[l][0],r)+" ,"}return a.merge(s,e.attrsMap),a.isEmptyObject(s)?a.isExist(e.val)?e.val:"":(a.isExist(e.val)&&("string"!=typeof e.val||""!==e.val&&e.val!==r.cdataPositionChar)&&(s+='"'+r.textNodeName+'" : '+(!0!==(n=e.val)&&!1!==n&&isNaN(n)?'"'+n+'"':n)),","===s[s.length-1]&&(s=s.substr(0,s.length-2)),s+"}")};e.convertToJsonString=function(t,e){return(e=i(e,n.defaultOptions,n.props)).indentBy=e.indentBy||"",s(t,e,0)}},function(t,e,r){"use strict";var a=r(0).buildOptions,i={attributeNamePrefix:"@_",attrNodeName:!1,textNodeName:"#text",ignoreAttributes:!0,cdataTagName:!1,cdataPositionChar:"\\c",format:!1,indentBy:"  ",supressEmptyNode:!1,tagValueProcessor:function(t){return t},attrValueProcessor:function(t){return t}},n=["attributeNamePrefix","attrNodeName","textNodeName","ignoreAttributes","cdataTagName","cdataPositionChar","format","indentBy","supressEmptyNode","tagValueProcessor","attrValueProcessor"];function s(t){this.options=a(t,i,n),this.options.ignoreAttributes||this.options.attrNodeName?this.isAttribute=function(){return!1}:(this.attrPrefixLen=this.options.attributeNamePrefix.length,this.isAttribute=g),this.options.cdataTagName?this.isCDATA=v:this.isCDATA=function(){return!1},this.replaceCDATAstr=o,this.replaceCDATAarr=u,this.options.format?(this.indentate=h,this.tagEndChar=">\n",this.newLine="\n"):(this.indentate=function(){return""},this.tagEndChar=">",this.newLine=""),this.options.supressEmptyNode?(this.buildTextNode=c,this.buildObjNode=d):(this.buildTextNode=f,this.buildObjNode=l),this.buildTextValNode=f,this.buildObjectNode=l}function o(t,e){return t=this.options.tagValueProcessor(""+t),""===this.options.cdataPositionChar||""===t?t+"<![CDATA["+e+"]]"+this.tagEndChar:t.replace(this.options.cdataPositionChar,"<![CDATA["+e+"]]"+this.tagEndChar)}function u(t,e){if(t=this.options.tagValueProcessor(""+t),""===this.options.cdataPositionChar||""===t)return t+"<![CDATA["+e.join("]]><![CDATA[")+"]]"+this.tagEndChar;for(var r in e)t=t.replace(this.options.cdataPositionChar,"<![CDATA["+e[r]+"]]>");return t+this.newLine}function l(t,e,r,a){return r&&!t.includes("<")?this.indentate(a)+"<"+e+r+">"+t+"</"+e+this.tagEndChar:this.indentate(a)+"<"+e+r+this.tagEndChar+t+this.indentate(a)+"</"+e+this.tagEndChar}function d(t,e,r,a){return""!==t?this.buildObjectNode(t,e,r,a):this.indentate(a)+"<"+e+r+"/"+this.tagEndChar}function f(t,e,r,a){return this.indentate(a)+"<"+e+r+">"+this.options.tagValueProcessor(t)+"</"+e+this.tagEndChar}function c(t,e,r,a){return""!==t?this.buildTextValNode(t,e,r,a):this.indentate(a)+"<"+e+r+"/"+this.tagEndChar}function h(t){return this.options.indentBy.repeat(t)}function g(t){return!!t.startsWith(this.options.attributeNamePrefix)&&t.substr(this.attrPrefixLen)}function v(t){return t===this.options.cdataTagName}s.prototype.parse=function(t){return this.j2x(t,0).val},s.prototype.j2x=function(t,e){for(var r="",a="",i=Object.keys(t),n=i.length,s=0;s<n;s++){var o=i[s];if(void 0===t[o]);else if(null===t[o])a+=this.indentate(e)+"<"+o+"/"+this.tagEndChar;else if(t[o]instanceof Date)a+=this.buildTextNode(t[o],o,"",e);else if("object"!=typeof t[o]){var u=this.isAttribute(o);u?r+=" "+u+'="'+this.options.attrValueProcessor(""+t[o])+'"':this.isCDATA(o)?t[this.options.textNodeName]?a+=this.replaceCDATAstr(t[this.options.textNodeName],t[o]):a+=this.replaceCDATAstr("",t[o]):o===this.options.textNodeName?t[this.options.cdataTagName]||(a+=this.options.tagValueProcessor(""+t[o])):a+=this.buildTextNode(t[o],o,"",e)}else if(Array.isArray(t[o]))if(this.isCDATA(o))a+=this.indentate(e),t[this.options.textNodeName]?a+=this.replaceCDATAarr(t[this.options.textNodeName],t[o]):a+=this.replaceCDATAarr("",t[o]);else for(var l=t[o].length,d=0;d<l;d++){var f=t[o][d];if(void 0===f);else if(null===f)a+=this.indentate(e)+"<"+o+"/"+this.tagEndChar;else if("object"==typeof f){var c=this.j2x(f,e+1);a+=this.buildObjNode(c.val,o,c.attrStr,e)}else a+=this.buildTextNode(f,o,"",e)}else if(this.options.attrNodeName&&o===this.options.attrNodeName)for(var h=Object.keys(t[o]),g=h.length,v=0;v<g;v++)r+=" "+h[v]+'="'+this.options.attrValueProcessor(""+t[o][h[v]])+'"';else{var p=this.j2x(t[o],e+1);a+=this.buildObjNode(p.val,o,p.attrStr,e)}}return{attrStr:r,val:a}},t.exports=s}]);
//# sourceMappingURL=parser.min.js.map