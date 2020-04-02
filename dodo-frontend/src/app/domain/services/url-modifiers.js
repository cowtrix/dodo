import { toPairs, join, map, compose } from 'ramda'

export const addParamsToUrl = (url, params) => {

	const paramsFormatted = compose(
		join('&'),
		map(param => param[0] + '=' + param[1]),
		toPairs
	)(params)

	return url + '?' + paramsFormatted
}