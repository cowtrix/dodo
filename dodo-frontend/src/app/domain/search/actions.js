import {
	SEARCH_FILTER_EVENTS,
	SEARCH_GET,
	SEARCH_FILTER_DISTANCE,
	SEARCH_FILTER_LOCATION
} from "./action-types"
import { apiAction } from "../factories/api-action"
import { SEARCH } from "../urls"

import { addParamsToUrl } from "../services"

const getLocation = result => navigator.geolocation.getCurrentPosition(result)

export const searchGet = (dispatch, params = {}) => {
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

export const searchFilterDistance = (dispatch, params) => {
	apiAction(dispatch, SEARCH_GET, addParamsToUrl(SEARCH, params))
	dispatch({
		type: SEARCH_FILTER_DISTANCE,
		payload: params.distance
	})
}

export const searchFilterLocation = (dispatch, params) => {
	apiAction(dispatch, SEARCH_GET, addParamsToUrl(SEARCH, params))
	dispatch({
		type: SEARCH_FILTER_DISTANCE,
		payload: params.latlong
	})
}
