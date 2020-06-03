import { path } from "ramda"
import { SEARCH_GET } from "./action-types"

export const searchResults = state =>
	path(["domain", "search", "searchResults"], state)


export const types = state =>
	path(["domain", "search", "searchParams", "types"], state)

export const distance = state => path(["domain", "search", "searchParams", "distance"], state)

export const latlong = state => path(["domain", "search", "searchParams", "latlong"], state)

export const search = state => path(["domain", "search", "searchParams", "search"], state)

export const searchParams = state => path(["domain", "search", "searchParams"], state)

export const isFetching = state =>
	path(["domain", "requests", SEARCH_GET, "isFetching"], state)
