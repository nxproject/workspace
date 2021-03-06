// https://www.notiflix.com/
(function (e, c) { "undefined" !== typeof module ? module.exports = c() : "function" === typeof define && "object" === typeof define.amd ? define(c) : this[e] = c() })("clipboard", function () {
    if ("undefined" === typeof document || !document.addEventListener) return null; var e = {}; e.copy = function () {
        function c() { d = !1; b = null; g && window.getSelection().removeAllRanges(); g = !1 } var d = !1, b = null, g = !1; document.addEventListener("copy", function (c) { if (d) { for (var g in b) c.clipboardData.setData(g, b[g]); c.preventDefault() } }); return function (f) {
            return new Promise(function (m,
                e) {
                    function k(b) { try { if (document.execCommand("copy")) c(), m(); else { if (b) throw c(), Error("Unable to copy. Perhaps it's not available in your browser?"); var d = document.getSelection(); if (!document.queryCommandEnabled("copy") && d.isCollapsed) { var f = document.createRange(); f.selectNodeContents(document.body); d.removeAllRanges(); d.addRange(f); g = !0 } k(!0) } } catch (a) { c(), e(a) } } d = !0; "string" === typeof f ? b = { "text/plain": f } : f instanceof Node ? b = { "text/html": (new XMLSerializer).serializeToString(f) } : f instanceof Object ?
                        b = f : e("Invalid data type. Must be string, DOM node, or an object mapping MIME types to strings."); k(!1)
            })
        }
    }(); e.paste = function () { var c = !1, d, b; document.addEventListener("paste", function (g) { if (c) { c = !1; g.preventDefault(); var f = d; d = null; f(g.clipboardData.getData(b)) } }); return function (g) { return new Promise(function (f, e) { c = !0; d = f; b = g || "text/plain"; try { document.execCommand("paste") || (c = !1, e(Error("Unable to paste. Pasting only works in Internet Explorer at the moment."))) } catch (h) { c = !1, e(Error(h)) } }) } }();
    "undefined" === typeof ClipboardEvent && "undefined" !== typeof window.clipboardData && "undefined" !== typeof window.clipboardData.setData && (function (c) {
        function d(a, b) { return function () { a.apply(b, arguments) } } function b(a) { if ("object" != typeof this) throw new TypeError("Promises must be constructed via new"); if ("function" != typeof a) throw new TypeError("not a function"); this._value = this._state = null; this._deferreds = []; l(a, d(f, this), d(e, this)) } function g(a) {
            var b = this; return null === this._state ? void this._deferreds.push(a) :
                void n(function () { var c = b._state ? a.onFulfilled : a.onRejected; if (null === c) return void (b._state ? a.resolve : a.reject)(b._value); var d; try { d = c(b._value) } catch (e) { return void a.reject(e) } a.resolve(d) })
        } function f(a) { try { if (a === this) throw new TypeError("A promise cannot be resolved with itself."); if (a && ("object" == typeof a || "function" == typeof a)) { var b = a.then; if ("function" == typeof b) return void l(d(b, a), d(f, this), d(e, this)) } this._state = !0; this._value = a; h.call(this) } catch (c) { e.call(this, c) } } function e(a) {
        this._state =
            !1; this._value = a; h.call(this)
        } function h() { for (var a = 0, b = this._deferreds.length; b > a; a++)g.call(this, this._deferreds[a]); this._deferreds = null } function k(a, b, c, d) { this.onFulfilled = "function" == typeof a ? a : null; this.onRejected = "function" == typeof b ? b : null; this.resolve = c; this.reject = d } function l(a, b, c) { var d = !1; try { a(function (a) { d || (d = !0, b(a)) }, function (a) { d || (d = !0, c(a)) }) } catch (e) { d || (d = !0, c(e)) } } var n = b.immediateFn || "function" == typeof setImmediate && setImmediate || function (a) { setTimeout(a, 1) }, p = Array.isArray ||
            function (a) { return "[object Array]" === Object.prototype.toString.call(a) }; b.prototype["catch"] = function (a) { return this.then(null, a) }; b.prototype.then = function (a, c) { var d = this; return new b(function (b, e) { g.call(d, new k(a, c, b, e)) }) }; b.all = function () {
                var a = Array.prototype.slice.call(1 === arguments.length && p(arguments[0]) ? arguments[0] : arguments); return new b(function (b, c) {
                    function d(f, g) {
                        try {
                            if (g && ("object" == typeof g || "function" == typeof g)) {
                                var h = g.then; if ("function" == typeof h) return void h.call(g, function (a) {
                                    d(f,
                                        a)
                                }, c)
                            } a[f] = g; 0 === --e && b(a)
                        } catch (k) { c(k) }
                    } if (0 === a.length) return b([]); for (var e = a.length, f = 0; f < a.length; f++)d(f, a[f])
                })
            }; b.resolve = function (a) { return a && "object" == typeof a && a.constructor === b ? a : new b(function (b) { b(a) }) }; b.reject = function (a) { return new b(function (b, c) { c(a) }) }; b.race = function (a) { return new b(function (b, c) { for (var d = 0, e = a.length; e > d; d++)a[d].then(b, c) }) }; "undefined" != typeof module && module.exports ? module.exports = b : c.Promise || (c.Promise = b)
    }(this), e.copy = function (c) {
        return new Promise(function (d,
            b) { if ("string" !== typeof c && !("text/plain" in c)) throw Error("You must provide a text/plain type."); window.clipboardData.setData("Text", "string" === typeof c ? c : c["text/plain"]) ? d() : b(Error("Copying was rejected.")) })
    }, e.paste = function () { return new Promise(function (c, d) { var b = window.clipboardData.getData("Text"); b ? c(b) : d(Error("Pasting was rejected.")) }) }); return e
});
