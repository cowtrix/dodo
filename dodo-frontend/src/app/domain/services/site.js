import { SITES } from "app/domain/urls"
import { api } from "./api-service"
import { memoize } from "./memoize"

export const fetchSite = memoize(async siteId => api(`${SITES}${siteId}`))
