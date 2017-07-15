if(typeof COMSCORE=="undefined")var COMSCORE={};if(typeof COMSCORE.SiteRecruit=="undefined"){COMSCORE.SiteRecruit={version:"4.6.3",configUrl:"broker-config.js",builderUrl:"builder.js",allowScriptCaching:false,CONSTANTS:{COOKIE_TYPE:{ALREADY_ASKED:1,DD_IN_PROGRESS:2},STATE_NAME:{IDLE:"IDLE",DDINPROGRESS:"DDINPROGRESS"}}};COMSCORE.SiteRecruit.Utils=function(){var a=COMSCORE.SiteRecruit;return{location:document.location.toString(),loadScript:function(b,d){if(d&&!a.allowScriptCaching)b=a.Utils.appendQueryParams(b,(new Date).getTime());var c=document.createElement("script");c.src=b;document.body.appendChild(c)},getBrowser:function(){var a={};a.name=navigator.appName;a.version=parseInt(navigator.appVersion,10);if(a.name=="Microsoft Internet Explorer")if(a.version>3){var b=navigator.userAgent.toLowerCase();if(b.indexOf("msie 5.0")==-1)a.ie=true;if(b.indexOf("msie 7")!=-1)a.ie7=true}if(a.name=="Netscape"||a.name=="Opera")if(a.version>4)a.mozilla=true;return a},fireBeacon:function(a){setTimeout(function(){if(a.indexOf("?")==-1)a+=(/\?/.test(a)?"&":"?")+(new Date).getTime();else a+="&"+(new Date).getTime();var b=new Image;b.src=a},1)},appendQueryParams:function(a,b){a==null||b==null;if(!a)return b;else{a=a.replace("?","")+"?";if(b)a+=b.toString().replace("?","");return a}},getRandom:function(k){var l=1e9;function d(b,d,c,f,e){var a=Math.floor(b/c);a=d*(b-a*c)-a*f;return Math.round(a<0?a+e:a)}for(var c=2147483563,n=2147483399,g=40014,m=40692,h=53668,o=52774,i=12211,p=3791,r=67108862,e=Math.round((new Date).getTime()%1e5)&2147483647,a=e,f=[32],b=0;b<19;b++)a=d(a,g,h,i,c);for(b=0;b<32;b++){a=d(a,g,h,i,c);f[31-b]=a}a=d(a,g,h,i,c);e=d(e,m,o,p,n);var q=Math.round((f[Math.floor(f[0]/r)]+e)%c),j=Math.floor(q/(c/(l+1)))/l;if(typeof k=="undefined")return j;else return Math.floor(j*(k+1))},getExecutingPath:function(d){for(var c=document.getElementsByTagName("script"),b=c.length-1;b>=0;b--){var a=c[b].src;this.scriptUrl=a;if(a.indexOf("/"+d)!=-1)return a.replace(/(.*)(\/.*)$/,"$1/")}},JSONDeserialize:function(a){try{if(a==="")a='""';eval("var p="+a+";");return p}catch(b){return null}},JSONSerialize:function(a){try{var c=typeof a;if(c!="object"||a===null){if(c=="string")a='"'+a+'"';return String(a)}else{var e,b,f=[],d=a&&a.constructor==Array;for(e in a){b=a[e];c=typeof b;if(c!="function"){if(c=="string")b='"'+b+'"';else if(c=="object"&&b!==null)b=this.JSONSerialize(b);f.push((d?"":'"'+e+'":')+String(b))}}return(d?"[":"{")+String(f)+(d?"]":"}")}}catch(g){return""}}}}();COMSCORE.SiteRecruit.Utils.UserPersistence={CONSTANTS:{STATE_NAME:{IDLE:"IDLE",DDINPROGRESS:"DDINPROGRESS"}},getCookieName:function(){var a;if(COMSCORE.SiteRecruit.Broker&&COMSCORE.SiteRecruit.Broker.config){a=COMSCORE.SiteRecruit.Broker.config.cookie;if(a.name)return a.name}return""},getDefaultCookieOptions:function(){var a={path:"/",domain:""};return a},getVendorId:function(){var a=1;return a},createCookie:function(d,b,a){b=escape(b);if(a.duration&&a.duration<0){var c=new Date;c.setTime(c.getTime()+a.duration*24*60*60*1e3);b+="; expires="+c.toGMTString()}else{var c=new Date;c.setTime(c.getTime()+10*365*24*60*60*1e3);b+="; expires="+c.toGMTString()}if(a.path)b+="; path="+a.path;if(a.domain)b+="; domain="+a.domain;if(a.secure)b+="; secure";document.cookie=d+"="+b;return true},getCookieValue:function(b){var a=document.cookie.match("(?:^|;)\\s*"+b+"=([^;]*)");return a?unescape(a[1]):false},removeCookie:function(b,a){a=a||{};a.duration=-999;this.createCookie(b,"",a)},createUserObj:function(b){var f=new Date,h=b.pid,i=b.url,e=this.CONSTANTS.STATE_NAME.IDLE;if(b.statename)e=b.statename;var c=f.getTime();if(b.timestamp)c=b.timestamp;var d=this.getCookieName();if(b.cookiename)d=b.cookiename;if(!b.cookieoptions)b.cookieoptions=this.getDefaultCookieOptions();var a={};a.version="4.6";a.state={};a.state.name=e;a.state.url=i;a.state.timestamp=c;a.lastinvited=c;a.userid=f.getTime().toString()+Math.floor(Math.random()*1e16).toString();a.vendorid=this.getVendorId();a.surveys=[];a.surveys.push(h);var g=COMSCORE.SiteRecruit.Utils.JSONSerialize(a);this.createCookie(d,g,b.cookieoptions);return a},setUserObj:function(b){var e=b.pid,j=b.url,k=new Date,d=this.CONSTANTS.STATE_NAME.IDLE;if(b.statename)d=b.statename;var c=k.getTime();if(b.timestamp)c=b.timestamp;var f=this.getCookieName();if(b.cookiename)f=b.cookiename;if(!b.cookieoptions)b.cookieoptions=this.getDefaultCookieOptions();var a=this.getUserObj(b);if(!a)this.createUserObj(b);else{var k=new Date;a.lastinvited=c;if(e){var g=false;for(i=0;i<a.surveys.length;i++)if(a.surveys[i]&&a.surveys[i].toLowerCase()==e.toLowerCase())g=true;g==false&&a.surveys.push(e);for(i=0;i<a.surveys.length;i++)a.surveys[i]==null&&a.surveys.splice(i,1)}if(d){a.state.name=d;a.state.url=j;a.state.timestamp=c}var h=COMSCORE.SiteRecruit.Utils.JSONSerialize(a);this.createCookie(f,h,b.cookieoptions)}return a},getUserObj:function(d){var c=this.getCookieName();if(d.cookiename)c=d.cookiename;var b=this.getCookieValue(c);if(b&&b!=""){var a=COMSCORE.SiteRecruit.Utils.JSONDeserialize(b);if(a&&a.version&&a.version=="4.6")return a}return null}};COMSCORE.SiteRecruit.DDKeepAlive=function(){var d=1e3,e=Math.random(),b,a=COMSCORE.SiteRecruit,c=a.Utils;return{start:function(){var c=this;b=setInterval(function(){if(a.Broker.isDDInProgress())c.setDDTrackerCookie();else c.stop()},d)},stop:function(){clearInterval(b)},setDDTrackerCookie:function(){var d=a.Broker.config.cookie,b={};b.cookieoptions={path:d.path,domain:d.domain};b.cookiename=d.name;b.url=escape(c.location);b.statename=a.CONSTANTS.STATE_NAME.DDINPROGRESS;if(COMSCORE.SiteRecruit.Builder&&COMSCORE.SiteRecruit.Builder.invitation&&COMSCORE.SiteRecruit.Builder.invitation.config)b.pid=COMSCORE.SiteRecruit.Builder.invitation.config.projectId;c.UserPersistence.setUserObj(b)}}}();COMSCORE.SiteRecruit.PagemapFinder=function(){var a,c=COMSCORE.SiteRecruit,b=c.Utils;return{getTotalFreq:function(){return a},find:function(j){var l=0,m,g=j,h=[],i=false;a=0;for(var f=0;g&&f<g.length;f++){var c=false,d=g[f];if(d){var k=new RegExp(d.m,"i");if(b.location.search(k)!=-1){if(d.halt){i=true;break}var e=g[f].prereqs;c=true;if(e){if(!this.isMatchContent(e.content))c=false;if(!this.isMatchCookie(e.cookie))c=false;if(!this.isMatchLanguage(e.language))c=false}}if(c){h.push(d);a+=d.f}}}if(i==true){h=null;a=0;return null}return this.choosePriority(h)},choose:function(c,e){for(var f=b.getRandom(e*100),d=0,a=0;a<c.length;a++){d+=c[a].f*100;if(f<=d)return c[a]}return null},choosePriority:function(c){for(var a=null,b=0;b<c.length;b++)if(a==null)a=c[b];else if(a.p<c[b].p)a=c[b];return a},isMatchContent:function(i){var f=true,h=0;while(f&&h<i.length){var d=false,b=false,a=i[h];if(a.element)for(var e=document.getElementsByTagName(a.element),c=0;c<e.length;c++){var g=a.elementValue;if(g&&g.length){if(e[c].innerHTML.search(g)!=-1)d=true}else d=true;if(a.attrib&&a.attrib.length){var j=e[c].attributes.getNamedItem(a.attrib);if(j)if(a.attribValue&&a.attribValue.length){if(j.value.search(a.attribValue)!=-1)b=true}else b=true}else b=true}if(!d||!b)f=false;h++}return f},isMatchCookie:function(e){var a=true,d=0;while(a&&d<e.length){var f=e[d],c=b.UserPersistence.getCookieValue(f.name);if(c&&c!==null){a=c.indexOf(f.value)!=-1?true:false;d++}else return false}return a},isMatchLanguage:function(b){var a=navigator.language||navigator.userLanguage;a=a.toLowerCase();if(!b)return true;if(a.indexOf(b)!=-1)return true;return false}}}();COMSCORE.SiteRecruit.Broker=function(){var a=COMSCORE.SiteRecruit,b=a.Utils;return{init:function(){a.browser=b.getBrowser();a.executingPath=b.getExecutingPath("broker.js");if(a.browser.ie||a.browser.mozilla)b.loadScript(a.executingPath+a.configUrl,true);else return},start:function(){this.init()},run:function(){if(this.config.objStoreElemName)if(a.browser.ie)COMSCORE.SiteRecruit.Utils.UserPersistence.initialize();else return;if(a.version!==this.config.version)return;this.isDDInProgress()&&this.processDDInProgress();if(!this.config.testMode||this.isDDInProgress()){var d={};d.cookiename=this.config.cookie.name;var c=b.UserPersistence.getUserObj(d),g=new Date,f=this.config.cookie.duration,e=g.getTime()-f*24*60*60*1e3;if(c)if(c.lastinvited>e)return}if(this.findPageMapping()){var h=b.getRandom();if(h<=a.PagemapFinder.getTotalFreq())this.pagemap&&this.loadBuilder();else return}else return},isDDInProgress:function(){var c=false,e={};e.cookiename=COMSCORE.SiteRecruit.Broker.config.cookie.name;var d=b.UserPersistence.getUserObj(e);if(d)if(d.state.name==a.CONSTANTS.STATE_NAME.DDINPROGRESS)c=true;return c},processDDInProgress:function(){a.DDKeepAlive.start()},findPageMapping:function(){this.pagemap=a.PagemapFinder.find(this.config.mapping);return this.pagemap},loadBuilder:function(){var c=a.executingPath+a.builderUrl;b.loadScript(c)}}}();COMSCORE.isDDInProgress=COMSCORE.SiteRecruit.Broker.isDDInProgress;COMSCORE.SiteRecruit.OnReady=function(){var a=COMSCORE.SiteRecruit,b=a.Utils;return{onload:function(){if(a.OnReady.done)return;a.OnReady.done=true;a.Broker.start();a.OnReady.timer&&clearInterval(a.OnReady.timer);document.addEventListener&&document.removeEventListener("DOMContentLoaded",a.OnReady.onload,false);window.ActiveXObject},listen:function(){if(/WebKit|khtml/i.test(navigator.userAgent))a.OnReady.timer=setInterval(function(){if(/loaded|complete/.test(document.readyState)){clearInterval(a.OnReady.timer);delete a.OnReady.timer;a.OnReady.onload()}},10);else if(document.addEventListener)document.addEventListener("DOMContentLoaded",a.OnReady.onload,false);else if(window.ActiveXObject)COMSCORE.SiteRecruit.OnReady.waitForLoad=setInterval(function(){try{document.documentElement.doScroll("left")}catch(a){return}COMSCORE.SiteRecruit.OnReady.waitForLoad=clearInterval(COMSCORE.SiteRecruit.OnReady.waitForLoad);COMSCORE.SiteRecruit.OnReady.onload()},1e3);else if(window.addEventListener)window.addEventListener("load",a.OnReady.onload,false);else if(window.attachEvent)return window.attachEvent("onload",a.OnReady.onload)},f:[],done:false,timer:null}}();COMSCORE.SiteRecruit.OnReady.listen()};