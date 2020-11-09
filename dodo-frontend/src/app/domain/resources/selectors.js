import { path } from "ramda"
import { RESOURCE_GET, RESOURCE_NOTIFICATIONS_GET } from "./action-types"

export const currentResource = state => path(["domain", "resources", "currentResource"], state)

export const currentNotifications = state => path(["domain", "resources", "currentNotifications"], state)

export const resourceTypes = state => path(["domain", "resources", "resources", "resourceTypes"], state)

export const homeVideo = state => path(["domain", "resources", "resources", "indexVideoEmbed"], state)

export const resourceError = state => path(["requests", RESOURCE_GET, "error"], state)

export const resourceFailed = state => path(["requests", RESOURCE_GET, "hasErrored"], state)

export const resourceLoading = state => path(["requests", RESOURCE_GET, "isFetching"], state)

export const notificationsLoading = state => path(["requests", RESOURCE_NOTIFICATIONS_GET, "isFetching"], state)
