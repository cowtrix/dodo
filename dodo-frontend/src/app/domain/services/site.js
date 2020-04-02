import { api } from "./api-service"
import { SITES } from "app/domain/urls"

export const fetchSite = siteId => api(`${SITES}${siteId}`)
