// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function setClientTimeZoneCookie() {
	try {
		var tz = (Intl && Intl.DateTimeFormat && Intl.DateTimeFormat().resolvedOptions().timeZone) || '';
		if (!tz) return;

		var match = document.cookie.match(/(?:^|; )tz=([^;]*)/);
		var current = match ? decodeURIComponent(match[1]) : null;
		if (current === tz) return;

		var expires = new Date(Date.now() + 365 * 24 * 60 * 60 * 1000).toUTCString();
		document.cookie = 'tz=' + encodeURIComponent(tz) + '; path=/; expires=' + expires + '; samesite=lax';
	} catch (_) {
		// ignore
	}
})();
