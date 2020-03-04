import { ALL_REBELLIONS_GET, REBELLION_GET } from "./action-types"
import { apiAction } from "../factories/api-action"
import { REBELLIONS } from '../urls'

export const allRebellionsGet = dispatch => apiAction(dispatch, ALL_REBELLIONS_GET, REBELLIONS)
export const rebellionGet = (dispatch, rebellion) => apiAction(dispatch, REBELLION_GET, REBELLIONS + rebellion)