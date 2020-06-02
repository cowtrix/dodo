import {
	SEARCH_FILTER_TYPES,
	SEARCH_GET,
	SEARCH_FILTER_DISTANCE,
	SEARCH_FILTER_LOCATION,
	SEARCH_FILTER_SEARCH
} from "./action-types"
import { apiAction } from "../factories/api-action"
import { SEARCH } from "../urls"

import { addParamsToUrl } from "../services"

export const searchGet = (dispatch, params) => {

	const parseTypes = params =>
		params.types ? ({
			...params,
			types: params.types.map(type => type.value)
		}) :
			params

	apiAction(dispatch, SEARCH_GET, addParamsToUrl(SEARCH, parseTypes(params)))
	params.distance &&
		dispatch({
			type: SEARCH_FILTER_DISTANCE,
			payload: params.distance
		})
	params.latlong &&
		dispatch({
			type: SEARCH_FILTER_LOCATION,
			payload: params.latlong
		})
	params.search &&
	dispatch({
		type: SEARCH_FILTER_SEARCH,
		payload: params.search
	})
	params.types.length &&
	dispatch({
		type: SEARCH_FILTER_TYPES,
		payload: params.types
	})
}

export const searchSetCurrentLocation = dispatch => {
	navigator.geolocation.getCurrentPosition(position => {
		const loc = position.coords
		const latLong = loc.latitude + "+" + loc.longitude
		dispatch({
			type: SEARCH_FILTER_LOCATION,
			payload: latLong
		})
	})
}
