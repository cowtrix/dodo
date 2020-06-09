import { path } from "ramda"
import { RESOURCE_GET } from "./action-types"

export const currentResource = state => path(["domain", "resources", "currentResource"], state)

export const resourceTypes = state => path(["domain", "resources", "resources", "resourceTypes"], state)

export const homeVideo = state => path(["domain", "resources", "resources", "indexVideoEmbed"], state)

export const resourceLoading = state =>
	path(["requests", RESOURCE_GET, "isFetching"], state)
