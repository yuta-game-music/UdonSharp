"use strict";(self.webpackChunkclient_sim=self.webpackChunkclient_sim||[]).push([[9596],{3905:function(e,t,n){n.d(t,{Zo:function(){return s},kt:function(){return f}});var r=n(7294);function o(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function a(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function l(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?a(Object(n),!0).forEach((function(t){o(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):a(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function i(e,t){if(null==e)return{};var n,r,o=function(e,t){if(null==e)return{};var n,r,o={},a=Object.keys(e);for(r=0;r<a.length;r++)n=a[r],t.indexOf(n)>=0||(o[n]=e[n]);return o}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(r=0;r<a.length;r++)n=a[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(o[n]=e[n])}return o}var c=r.createContext({}),u=function(e){var t=r.useContext(c),n=t;return e&&(n="function"==typeof e?e(t):l(l({},t),e)),n},s=function(e){var t=u(e.components);return r.createElement(c.Provider,{value:t},e.children)},p={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},m=r.forwardRef((function(e,t){var n=e.components,o=e.mdxType,a=e.originalType,c=e.parentName,s=i(e,["components","mdxType","originalType","parentName"]),m=u(n),f=o,g=m["".concat(c,".").concat(f)]||m[f]||p[f]||a;return n?r.createElement(g,l(l({ref:t},s),{},{components:n})):r.createElement(g,l({ref:t},s))}));function f(e,t){var n=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var a=n.length,l=new Array(a);l[0]=m;var i={};for(var c in t)hasOwnProperty.call(t,c)&&(i[c]=t[c]);i.originalType=e,i.mdxType="string"==typeof e?e:o,l[1]=i;for(var u=2;u<a;u++)l[u]=n[u];return r.createElement.apply(null,l)}return r.createElement.apply(null,n)}m.displayName="MDXCreateElement"},7890:function(e,t,n){n.r(t),n.d(t,{assets:function(){return s},contentTitle:function(){return c},default:function(){return f},frontMatter:function(){return i},metadata:function(){return u},toc:function(){return p}});var r=n(7462),o=n(3366),a=(n(7294),n(3905)),l=["components"],i={slug:"release-1.1.1",title:"Release 1.1.1",date:new Date("2022-08-26T00:00:00.000Z"),authors:["momo"],tags:["release"],draft:!1},c=void 0,u={permalink:"/news/release-1.1.1",source:"@site/news/releases/release-1.1.1.md",title:"Release 1.1.1",description:"Changelog",date:"2022-08-26T00:00:00.000Z",formattedDate:"August 26, 2022",tags:[{label:"release",permalink:"/news/tags/release"}],truncated:!1,authors:[{name:"Momo the Monster",title:"VRChat Developer",url:"https://github.com/momo-the-monster",imageURL:"https://github.com/momo-the-monster.png",key:"momo"}],frontMatter:{slug:"release-1.1.1",title:"Release 1.1.1",date:"2022-08-26T00:00:00.000Z",authors:["momo"],tags:["release"],draft:!1},nextItem:{title:"Release 1.0.0b12",permalink:"/news/release-1.0.0b12"}},s={authorsImageUrls:[void 0]},p=[{value:"Changelog",id:"changelog",level:2}],m={toc:p};function f(e){var t=e.components,n=(0,o.Z)(e,l);return(0,a.kt)("wrapper",(0,r.Z)({},m,n,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h2",{id:"changelog"},"Changelog"),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},"Fixes Locator to avoid spamming this Error on first project open: ",(0,a.kt)("inlineCode",{parentName:"li"},"Exception: Could not find UdonSharp locator, make sure you have installed U# following the install instructions.")),(0,a.kt)("li",{parentName:"ul"},"Fixes file watcher breaking on non-Windows systems, contributed by anatawa12 (",(0,a.kt)("a",{parentName:"li",href:"https://github.com/vrchat-community/UdonSharp/pull/47"},"https://github.com/vrchat-community/UdonSharp/pull/47"),")")))}f.isMDXComponent=!0}}]);