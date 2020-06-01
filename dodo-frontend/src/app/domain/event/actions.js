import { EVENT_GET, EVENT_TYPES_GET } from "./action-types"
import { apiAction } from "../factories/api-action"
import { API_URL } from "../urls"

export const eventGet = (dispatch, eventType, event) =>
	apiAction(dispatch, EVENT_GET, API_URL + eventType + "/" + event)

const setSearchEvents = (result) => {
	console.log(result)
}

export const eventTypesGet = (dispatch) =>
	apiAction(dispatch, EVENT_TYPES_GET, API_URL)
