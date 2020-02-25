import { ALL_REBELLIONS_GET } from "./action-types"
import { apiAction } from "../factories/api-action"
import { ALL_REBELLIONS_GET as ALL_REBELLIONS_GET_URL } from '../urls'

export const allRebellionsGet = dispatch => apiAction(dispatch, ALL_REBELLIONS_GET, ALL_REBELLIONS_GET_URL)