!function(e,t){"object"==typeof exports&&"object"==typeof module?module.exports=t():"function"==typeof define&&define.amd?define("cronstrue",[],t):"object"==typeof exports?exports.cronstrue=t():e.cronstrue=t()}("undefined"!=typeof self?self:this,function(){return r={},i.m=n=[function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.ExpressionDescriptor=void 0;var g=n(1),o=n(2),r=(p.toString=function(e,t){var n=void 0===t?{}:t,r=n.throwExceptionOnParseError,i=void 0===r||r,o=n.verbose,s=void 0!==o&&o,a=n.dayOfWeekStartIndexZero,u=void 0===a||a,c=n.use24HourTimeFormat,f=n.locale;return new p(e,{throwExceptionOnParseError:i,verbose:s,dayOfWeekStartIndexZero:u,use24HourTimeFormat:c,locale:void 0===f?"en":f}).getFullDescription()},p.initialize=function(e){p.specialCharacters=["/","-",",","*"],e.load(p.locales)},p.prototype.getFullDescription=function(){var t="";try{var e=new o.CronParser(this.expression,this.options.dayOfWeekStartIndexZero);this.expressionParts=e.parse();var n=this.getTimeOfDayDescription(),r=this.getDayOfMonthDescription(),i=this.getMonthDescription();t+=n+r+this.getDayOfWeekDescription()+i+this.getYearDescription(),t=(t=this.transformVerbosity(t,this.options.verbose)).charAt(0).toLocaleUpperCase()+t.substr(1)}catch(e){if(this.options.throwExceptionOnParseError)throw""+e;t=this.i18n.anErrorOccuredWhenGeneratingTheExpressionD()}return t},p.prototype.getTimeOfDayDescription=function(){var e=this.expressionParts[0],t=this.expressionParts[1],n=this.expressionParts[2],r="";if(g.StringUtilities.containsAny(t,p.specialCharacters)||g.StringUtilities.containsAny(n,p.specialCharacters)||g.StringUtilities.containsAny(e,p.specialCharacters))if(e||!(-1<t.indexOf("-"))||-1<t.indexOf(",")||-1<t.indexOf("/")||g.StringUtilities.containsAny(n,p.specialCharacters))if(!e&&-1<n.indexOf(",")&&-1==n.indexOf("-")&&-1==n.indexOf("/")&&!g.StringUtilities.containsAny(t,p.specialCharacters)){var i=n.split(",");r+=this.i18n.at();for(var o=0;o<i.length;o++)r+=" ",r+=this.formatTime(i[o],t,""),o<i.length-2&&(r+=","),o==i.length-2&&(r+=this.i18n.spaceAnd())}else{var s=this.getSecondsDescription(),a=this.getMinutesDescription(),u=this.getHoursDescription();if(0<(r+=s).length&&0<a.length&&(r+=", "),r+=a,a===u)return r;0<r.length&&0<u.length&&(r+=", "),r+=u}else{var c=t.split("-");r+=g.StringUtilities.format(this.i18n.everyMinuteBetweenX0AndX1(),this.formatTime(n,c[0],""),this.formatTime(n,c[1],""))}else r+=this.i18n.atSpace()+this.formatTime(n,t,e);return r},p.prototype.getSecondsDescription=function(){var t=this;return this.getSegmentDescription(this.expressionParts[0],this.i18n.everySecond(),function(e){return e},function(e){return g.StringUtilities.format(t.i18n.everyX0Seconds(),e)},function(e){return t.i18n.secondsX0ThroughX1PastTheMinute()},function(e){return"0"==e?"":parseInt(e)<20?t.i18n.atX0SecondsPastTheMinute():t.i18n.atX0SecondsPastTheMinuteGt20()||t.i18n.atX0SecondsPastTheMinute()})},p.prototype.getMinutesDescription=function(){var t=this,n=this.expressionParts[0],r=this.expressionParts[2];return this.getSegmentDescription(this.expressionParts[1],this.i18n.everyMinute(),function(e){return e},function(e){return g.StringUtilities.format(t.i18n.everyX0Minutes(),e)},function(e){return t.i18n.minutesX0ThroughX1PastTheHour()},function(e){try{return"0"==e&&-1==r.indexOf("/")&&""==n?t.i18n.everyHour():parseInt(e)<20?t.i18n.atX0MinutesPastTheHour():t.i18n.atX0MinutesPastTheHourGt20()||t.i18n.atX0MinutesPastTheHour()}catch(e){return t.i18n.atX0MinutesPastTheHour()}})},p.prototype.getHoursDescription=function(){var t=this,e=this.expressionParts[2];return this.getSegmentDescription(e,this.i18n.everyHour(),function(e){return t.formatTime(e,"0","")},function(e){return g.StringUtilities.format(t.i18n.everyX0Hours(),e)},function(e){return t.i18n.betweenX0AndX1()},function(e){return t.i18n.atX0()})},p.prototype.getDayOfWeekDescription=function(){var r=this,n=this.i18n.daysOfTheWeek();return"*"==this.expressionParts[5]?"":this.getSegmentDescription(this.expressionParts[5],this.i18n.commaEveryDay(),function(e){var t=e;return-1<e.indexOf("#")?t=e.substr(0,e.indexOf("#")):-1<e.indexOf("L")&&(t=t.replace("L","")),n[parseInt(t)]},function(e){return 1==parseInt(e)?"":g.StringUtilities.format(r.i18n.commaEveryX0DaysOfTheWeek(),e)},function(e){return r.i18n.commaX0ThroughX1()},function(e){var t=null;if(-1<e.indexOf("#")){var n=null;switch(e.substring(e.indexOf("#")+1)){case"1":n=r.i18n.first();break;case"2":n=r.i18n.second();break;case"3":n=r.i18n.third();break;case"4":n=r.i18n.fourth();break;case"5":n=r.i18n.fifth()}t=r.i18n.commaOnThe()+n+r.i18n.spaceX0OfTheMonth()}else t=-1<e.indexOf("L")?r.i18n.commaOnTheLastX0OfTheMonth():"*"!=r.expressionParts[3]?r.i18n.commaAndOnX0():r.i18n.commaOnlyOnX0();return t})},p.prototype.getMonthDescription=function(){var t=this,n=this.i18n.monthsOfTheYear();return this.getSegmentDescription(this.expressionParts[4],"",function(e){return n[parseInt(e)-1]},function(e){return 1==parseInt(e)?"":g.StringUtilities.format(t.i18n.commaEveryX0Months(),e)},function(e){return t.i18n.commaMonthX0ThroughMonthX1()||t.i18n.commaX0ThroughX1()},function(e){return t.i18n.commaOnlyInMonthX0?t.i18n.commaOnlyInMonthX0():t.i18n.commaOnlyInX0()})},p.prototype.getDayOfMonthDescription=function(){var t=this,e=null,n=this.expressionParts[3];switch(n){case"L":e=this.i18n.commaOnTheLastDayOfTheMonth();break;case"WL":case"LW":e=this.i18n.commaOnTheLastWeekdayOfTheMonth();break;default:var r=n.match(/(\d{1,2}W)|(W\d{1,2})/);if(r){var i=parseInt(r[0].replace("W","")),o=1==i?this.i18n.firstWeekday():g.StringUtilities.format(this.i18n.weekdayNearestDayX0(),i.toString());e=g.StringUtilities.format(this.i18n.commaOnTheX0OfTheMonth(),o);break}var s=n.match(/L-(\d{1,2})/);if(s){var a=s[1];e=g.StringUtilities.format(this.i18n.commaDaysBeforeTheLastDayOfTheMonth(),a);break}if("*"==n&&"*"!=this.expressionParts[5])return"";e=this.getSegmentDescription(n,this.i18n.commaEveryDay(),function(e){return"L"==e?t.i18n.lastDay():t.i18n.dayX0?g.StringUtilities.format(t.i18n.dayX0(),e):e},function(e){return"1"==e?t.i18n.commaEveryDay():t.i18n.commaEveryX0Days()},function(e){return t.i18n.commaBetweenDayX0AndX1OfTheMonth()},function(e){return t.i18n.commaOnDayX0OfTheMonth()})}return e},p.prototype.getYearDescription=function(){var t=this;return this.getSegmentDescription(this.expressionParts[6],"",function(e){return/^\d+$/.test(e)?new Date(parseInt(e),1).getFullYear().toString():e},function(e){return g.StringUtilities.format(t.i18n.commaEveryX0Years(),e)},function(e){return t.i18n.commaYearX0ThroughYearX1()||t.i18n.commaX0ThroughX1()},function(e){return t.i18n.commaOnlyInYearX0?t.i18n.commaOnlyInYearX0():t.i18n.commaOnlyInX0()})},p.prototype.getSegmentDescription=function(e,t,n,r,i,o){var s=null,a=-1<e.indexOf("/"),u=-1<e.indexOf("-"),c=-1<e.indexOf(",");if(e)if("*"===e)s=t;else if(a||u||c)if(c){for(var f=e.split(","),p="",h=0;h<f.length;h++)if(0<h&&2<f.length&&(p+=",",h<f.length-1&&(p+=" ")),0<h&&1<f.length&&(h==f.length-1||2==f.length)&&(p+=this.i18n.spaceAnd()+" "),-1<f[h].indexOf("/")||-1<f[h].indexOf("-")){var l=-1<f[h].indexOf("-")&&-1==f[h].indexOf("/"),y=this.getSegmentDescription(f[h],t,n,r,l?this.i18n.commaX0ThroughX1:i,o);l&&(y=y.replace(", ","")),p+=y}else p+=a?this.getSegmentDescription(f[h],t,n,r,i,o):n(f[h]);s=a?p:g.StringUtilities.format(o(e),p)}else if(a){if(f=e.split("/"),s=g.StringUtilities.format(r(f[1]),f[1]),-1<f[0].indexOf("-")){var d=this.generateRangeSegmentDescription(f[0],i,n);0!=d.indexOf(", ")&&(s+=", "),s+=d}else if(-1==f[0].indexOf("*")){var m=g.StringUtilities.format(o(f[0]),n(f[0]));m=m.replace(", ",""),s+=g.StringUtilities.format(this.i18n.commaStartingX0(),m)}}else u&&(s=this.generateRangeSegmentDescription(e,i,n));else s=g.StringUtilities.format(o(e),n(e));else s="";return s},p.prototype.generateRangeSegmentDescription=function(e,t,n){var r="",i=e.split("-"),o=n(i[0]),s=n(i[1]);s=s.replace(":00",":59");var a=t(e);return r+=g.StringUtilities.format(a,o,s)},p.prototype.formatTime=function(e,t,n){var r=parseInt(e),i="",o=!1;this.options.use24HourTimeFormat||(i=(o=this.i18n.setPeriodBeforeTime&&this.i18n.setPeriodBeforeTime())?this.getPeriod(r)+" ":" "+this.getPeriod(r),12<r&&(r-=12),0===r&&(r=12));var s=t,a="";return n&&(a=":"+("00"+n).substring(n.length)),""+(o?i:"")+("00"+r.toString()).substring(r.toString().length)+":"+("00"+s.toString()).substring(s.toString().length)+a+(o?"":i)},p.prototype.transformVerbosity=function(e,t){return t||(e=(e=(e=(e=e.replace(new RegExp(", "+this.i18n.everyMinute(),"g"),"")).replace(new RegExp(", "+this.i18n.everyHour(),"g"),"")).replace(new RegExp(this.i18n.commaEveryDay(),"g"),"")).replace(/\, ?$/,"")),e},p.prototype.getPeriod=function(e){return 12<=e?this.i18n.pm&&this.i18n.pm()||"PM":this.i18n.am&&this.i18n.am()||"AM"},p.locales={},p);function p(e,t){this.expression=e,this.options=t,this.expressionParts=new Array(5),p.locales[t.locale]?this.i18n=p.locales[t.locale]:(console.warn("Locale '"+t.locale+"' could not be found; falling back to 'en'."),this.i18n=p.locales.en),void 0===t.use24HourTimeFormat&&(t.use24HourTimeFormat=this.i18n.use24HourTimeFormatByDefault())}t.ExpressionDescriptor=r},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.StringUtilities=void 0;var r=(i.format=function(e){for(var t=[],n=1;n<arguments.length;n++)t[n-1]=arguments[n];return e.replace(/%s/g,function(){return t.shift()})},i.containsAny=function(t,e){return e.some(function(e){return-1<t.indexOf(e)})},i);function i(){}t.StringUtilities=r},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.CronParser=void 0;var r=n(3),i=(o.prototype.parse=function(){var e=this.extractParts(this.expression);return this.normalize(e),this.validate(e),e},o.prototype.extractParts=function(e){if(!this.expression)throw new Error("Expression is empty");var t=e.trim().split(/[ ]+/);if(t.length<5)throw new Error("Expression has only "+t.length+" part"+(1==t.length?"":"s")+". At least 5 parts are required.");if(5==t.length)t.unshift(""),t.push("");else if(6==t.length)/\d{4}$/.test(t[5])||"?"==t[4]||"?"==t[2]?t.unshift(""):t.push("");else if(7<t.length)throw new Error("Expression has "+t.length+" parts; too many!");return t},o.prototype.normalize=function(e){var r=this;if(e[3]=e[3].replace("?","*"),e[5]=e[5].replace("?","*"),e[2]=e[2].replace("?","*"),0==e[0].indexOf("0/")&&(e[0]=e[0].replace("0/","*/")),0==e[1].indexOf("0/")&&(e[1]=e[1].replace("0/","*/")),0==e[2].indexOf("0/")&&(e[2]=e[2].replace("0/","*/")),0==e[3].indexOf("1/")&&(e[3]=e[3].replace("1/","*/")),0==e[4].indexOf("1/")&&(e[4]=e[4].replace("1/","*/")),0==e[6].indexOf("1/")&&(e[6]=e[6].replace("1/","*/")),e[5]=e[5].replace(/(^\d)|([^#/\s]\d)/g,function(e){var t=e.replace(/\D/,""),n=t;return r.dayOfWeekStartIndexZero?"7"==t&&(n="0"):n=(parseInt(t)-1).toString(),e.replace(t,n)}),"L"==e[5]&&(e[5]="6"),"?"==e[3]&&(e[3]="*"),-1<e[3].indexOf("W")&&(-1<e[3].indexOf(",")||-1<e[3].indexOf("-")))throw new Error("The 'W' character can be specified only when the day-of-month is a single day, not a range or list of days.");var t={SUN:0,MON:1,TUE:2,WED:3,THU:4,FRI:5,SAT:6};for(var n in t)e[5]=e[5].replace(new RegExp(n,"gi"),t[n].toString());var i={JAN:1,FEB:2,MAR:3,APR:4,MAY:5,JUN:6,JUL:7,AUG:8,SEP:9,OCT:10,NOV:11,DEC:12};for(var o in i)e[4]=e[4].replace(new RegExp(o,"gi"),i[o].toString());"0"==e[0]&&(e[0]=""),/\*|\-|\,|\//.test(e[2])||!/\*|\//.test(e[1])&&!/\*|\//.test(e[0])||(e[2]+="-"+e[2]);for(var s=0;s<e.length;s++)if(-1!=e[s].indexOf(",")&&(e[s]=e[s].split(",").filter(function(e){return""!==e}).join(",")||"*"),"*/1"==e[s]&&(e[s]="*"),-1<e[s].indexOf("/")&&!/^\*|\-|\,/.test(e[s])){var a=null;switch(s){case 4:a="12";break;case 5:a="6";break;case 6:a="9999";break;default:a=null}if(null!=a){var u=e[s].split("/");e[s]=u[0]+"-"+a+"/"+u[1]}}},o.prototype.validate=function(e){this.assertNoInvalidCharacters("DOW",e[5]),this.assertNoInvalidCharacters("DOM",e[3]),this.validateRange(e)},o.prototype.validateRange=function(e){r.default.secondRange(e[0]),r.default.minuteRange(e[1]),r.default.hourRange(e[2]),r.default.dayOfMonthRange(e[3]),r.default.monthRange(e[4]),r.default.dayOfWeekRange(e[5])},o.prototype.assertNoInvalidCharacters=function(e,t){var n=t.match(/[A-KM-VX-Z]+/gi);if(n&&n.length)throw new Error(e+" part contains invalid values: '"+n.toString()+"'")},o);function o(e,t){void 0===t&&(t=!0),this.expression=e,this.dayOfWeekStartIndexZero=t}t.CronParser=i},function(e,t,n){"use strict";function i(e,t){if(!e)throw new Error(t)}Object.defineProperty(t,"__esModule",{value:!0});var r=(o.secondRange=function(e){for(var t=e.split(","),n=0;n<t.length;n++)if(!isNaN(parseInt(t[n],10))){var r=parseInt(t[n],10);i(0<=r&&r<=59,"seconds part must be >= 0 and <= 59")}},o.minuteRange=function(e){for(var t=e.split(","),n=0;n<t.length;n++)if(!isNaN(parseInt(t[n],10))){var r=parseInt(t[n],10);i(0<=r&&r<=59,"minutes part must be >= 0 and <= 59")}},o.hourRange=function(e){for(var t=e.split(","),n=0;n<t.length;n++)if(!isNaN(parseInt(t[n],10))){var r=parseInt(t[n],10);i(0<=r&&r<=23,"hours part must be >= 0 and <= 23")}},o.dayOfMonthRange=function(e){for(var t=e.split(","),n=0;n<t.length;n++)if(!isNaN(parseInt(t[n],10))){var r=parseInt(t[n],10);i(1<=r&&r<=31,"DOM part must be >= 1 and <= 31")}},o.monthRange=function(e){for(var t=e.split(","),n=0;n<t.length;n++)if(!isNaN(parseInt(t[n],10))){var r=parseInt(t[n],10);i(1<=r&&r<=12,"month part must be >= 1 and <= 12")}},o.dayOfWeekRange=function(e){for(var t=e.split(","),n=0;n<t.length;n++)if(!isNaN(parseInt(t[n],10))){var r=parseInt(t[n],10);i(0<=r&&r<=6,"DOW part must be >= 0 and <= 6")}},o);function o(){}t.default=r},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.en=void 0;var r=(i.prototype.atX0SecondsPastTheMinuteGt20=function(){return null},i.prototype.atX0MinutesPastTheHourGt20=function(){return null},i.prototype.commaMonthX0ThroughMonthX1=function(){return null},i.prototype.commaYearX0ThroughYearX1=function(){return null},i.prototype.use24HourTimeFormatByDefault=function(){return!1},i.prototype.anErrorOccuredWhenGeneratingTheExpressionD=function(){return"An error occured when generating the expression description.  Check the cron expression syntax."},i.prototype.everyMinute=function(){return"every minute"},i.prototype.everyHour=function(){return"every hour"},i.prototype.atSpace=function(){return"At "},i.prototype.everyMinuteBetweenX0AndX1=function(){return"Every minute between %s and %s"},i.prototype.at=function(){return"At"},i.prototype.spaceAnd=function(){return" and"},i.prototype.everySecond=function(){return"every second"},i.prototype.everyX0Seconds=function(){return"every %s seconds"},i.prototype.secondsX0ThroughX1PastTheMinute=function(){return"seconds %s through %s past the minute"},i.prototype.atX0SecondsPastTheMinute=function(){return"at %s seconds past the minute"},i.prototype.everyX0Minutes=function(){return"every %s minutes"},i.prototype.minutesX0ThroughX1PastTheHour=function(){return"minutes %s through %s past the hour"},i.prototype.atX0MinutesPastTheHour=function(){return"at %s minutes past the hour"},i.prototype.everyX0Hours=function(){return"every %s hours"},i.prototype.betweenX0AndX1=function(){return"between %s and %s"},i.prototype.atX0=function(){return"at %s"},i.prototype.commaEveryDay=function(){return", every day"},i.prototype.commaEveryX0DaysOfTheWeek=function(){return", every %s days of the week"},i.prototype.commaX0ThroughX1=function(){return", %s through %s"},i.prototype.first=function(){return"first"},i.prototype.second=function(){return"second"},i.prototype.third=function(){return"third"},i.prototype.fourth=function(){return"fourth"},i.prototype.fifth=function(){return"fifth"},i.prototype.commaOnThe=function(){return", on the "},i.prototype.spaceX0OfTheMonth=function(){return" %s of the month"},i.prototype.lastDay=function(){return"the last day"},i.prototype.commaOnTheLastX0OfTheMonth=function(){return", on the last %s of the month"},i.prototype.commaOnlyOnX0=function(){return", only on %s"},i.prototype.commaAndOnX0=function(){return", and on %s"},i.prototype.commaEveryX0Months=function(){return", every %s months"},i.prototype.commaOnlyInX0=function(){return", only in %s"},i.prototype.commaOnTheLastDayOfTheMonth=function(){return", on the last day of the month"},i.prototype.commaOnTheLastWeekdayOfTheMonth=function(){return", on the last weekday of the month"},i.prototype.commaDaysBeforeTheLastDayOfTheMonth=function(){return", %s days before the last day of the month"},i.prototype.firstWeekday=function(){return"first weekday"},i.prototype.weekdayNearestDayX0=function(){return"weekday nearest day %s"},i.prototype.commaOnTheX0OfTheMonth=function(){return", on the %s of the month"},i.prototype.commaEveryX0Days=function(){return", every %s days"},i.prototype.commaBetweenDayX0AndX1OfTheMonth=function(){return", between day %s and %s of the month"},i.prototype.commaOnDayX0OfTheMonth=function(){return", on day %s of the month"},i.prototype.commaEveryHour=function(){return", every hour"},i.prototype.commaEveryX0Years=function(){return", every %s years"},i.prototype.commaStartingX0=function(){return", starting %s"},i.prototype.daysOfTheWeek=function(){return["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"]},i.prototype.monthsOfTheYear=function(){return["January","February","March","April","May","June","July","August","September","October","November","December"]},i);function i(){}t.en=r},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.toString=void 0;var r=n(0),i=n(6);r.ExpressionDescriptor.initialize(new i.enLocaleLoader),t.default=r.ExpressionDescriptor;var o=r.ExpressionDescriptor.toString;t.toString=o},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.enLocaleLoader=void 0;var r=n(4),i=(o.prototype.load=function(e){e.en=new r.en},o);function o(){}t.enLocaleLoader=i}],i.c=r,i.d=function(e,t,n){i.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:n})},i.r=function(e){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},i.t=function(t,e){if(1&e&&(t=i(t)),8&e)return t;if(4&e&&"object"==typeof t&&t&&t.__esModule)return t;var n=Object.create(null);if(i.r(n),Object.defineProperty(n,"default",{enumerable:!0,value:t}),2&e&&"string"!=typeof t)for(var r in t)i.d(n,r,function(e){return t[e]}.bind(null,r));return n},i.n=function(e){var t=e&&e.__esModule?function(){return e.default}:function(){return e};return i.d(t,"a",t),t},i.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},i.p="",i(i.s=5);function i(e){if(r[e])return r[e].exports;var t=r[e]={i:e,l:!1,exports:{}};return n[e].call(t.exports,t,t.exports,i),t.l=!0,t.exports}var n,r});