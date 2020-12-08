import { RESOURCE_GET, RESOURCE_NOTIFICATIONS_GET, RESOURCES_GET, RESOURCE_JOIN } from "./action-types"
import { apiAction } from "../factories/api-action"
import { API_URL } from "../urls"

export const resourceGet = (dispatch, resourceType, slug, cb) =>
	apiAction(dispatch, RESOURCE_GET, API_URL + resourceType + "/" + slug, cb)

export const eventTypesGet = (dispatch) =>
	apiAction(dispatch, RESOURCES_GET, API_URL)

export const notificationsGet = (dispatch, eventType, slug, page=1, cb) =>
	apiAction(dispatch, RESOURCE_NOTIFICATIONS_GET, API_URL + eventType + "/notifications/" + slug + '/?page=' + page, cb)

export const subscribeResource = (dispatch, resourceType, slug, subscribe, body) =>
	apiAction(dispatch, RESOURCE_JOIN, API_URL + resourceType + "/" + slug + '/' + subscribe, () => resourceGet(dispatch, resourceType, slug), false, 'post', body)

