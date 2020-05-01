import {
	SEARCH_FILTER_EVENTS,
	SEARCH_FILTER_DATE,
	SEARCH_GET,
	SEARCH_FILTER_DISTANCE,
	SEARCH_FILTER_LOCATION
} from "./action-types"
import { apiAction } from "../factories/api-action"
import { SEARCH } from "../urls"

import { addParamsToUrl } from "../services"

export const searchGet = (dispatch, params) => {
	params.distance &&
		params.latlong &&
		apiAction(dispatch, SEARCH_GET, addParamsToUrl(SEARCH, params))
	dispatch({
		type: SEARCH_FILTER_DISTANCE,
		payload: params.distance
	})
	dispatch({
		type: SEARCH_FILTER_LOCATION,
		payload: params.latlong
	})
}

export const searchFilterEvents = payload => ({
	type: SEARCH_FILTER_EVENTS,
	payload
})

export const searchFilterDate = payload => ({
	type: SEARCH_FILTER_DATE,
	payload
})

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
