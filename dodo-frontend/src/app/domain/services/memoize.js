export const memoize = method => {
	let cache = {}
	return function(...args) {
		let strArgs = JSON.stringify(args)

		return cache[strArgs]
			? cache[strArgs]
			: method(...args).then(result => {
					cache[strArgs] = result
					return result
			  })
	}
}
