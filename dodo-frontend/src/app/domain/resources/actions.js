import { RESOURCE_GET, RESOURCE_NOTIFICATIONS_GET, RESOURCES_GET, RESOURCE_JOIN } from "./action-types"
import { apiAction } from "../factories/api-action"
import { API_URL } from "../urls"
import { getLoggedInUser } from '../user/actions'


export const resourceGet = (dispatch, resourceType, resourceId, cb) =>
	apiAction(dispatch, RESOURCE_GET, API_URL + resourceType + "/" + resourceId, cb)

export const eventTypesGet = (dispatch) =>
	apiAction(dispatch, RESOURCES_GET, API_URL)

export const notificationsGet = (dispatch, eventType, event, page=1, cb) =>
	apiAction(dispatch, RESOURCE_NOTIFICATIONS_GET, API_URL + eventType + "/notifications/" + event + '/?page=' + page, cb)

export const subscribeResource = (dispatch, resourceType, resourceId, subscribe, body) =>
	apiAction(dispatch, RESOURCE_JOIN, API_URL + resourceType + "/" + resourceId + '/' + subscribe, () => getLoggedInUser(dispatch), false, 'post', body)

