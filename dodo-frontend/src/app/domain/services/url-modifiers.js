import { toPairs, join, map, compose, filter } from 'ramda'

export const addParamsToUrl = (url, params) => {

	const paramsFormatted = compose(
		join('&'),
		map(param => param[0] + '=' + param[1]),
		filter(param => param[1].length ),
		toPairs
	)(params)

	return url + '?' + paramsFormatted
}