import { EVENT_GET } from "./action-types"
import { apiAction } from "../factories/api-action"
import { API_URL } from "../urls"

export const eventGet = (dispatch, eventType, event) =>
	apiAction(dispatch, EVENT_GET, API_URL + eventType + "/" + event)
