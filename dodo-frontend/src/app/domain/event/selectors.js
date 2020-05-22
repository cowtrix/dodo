import { path } from "ramda"
import { EVENT_GET } from "./action-types"

export const currentEvent = state => path(["domain", "currentEvent"], state)

export const eventLoading = state =>
	path(["domain", "requests", EVENT_GET, "isFetching"], state)
