export const memoize = method => {
	let cache = {}
	return async function(...args) {
		let strArgs = JSON.stringify(args)
		cache[strArgs] = cache[strArgs] || method(...args)
		return cache[strArgs]
	}
}
