import { ALL_LOCAL_GROUPS_GET, LOCAL_GROUP_GET } from "./action-types"
import { apiAction } from "../factories/api-action"
import { LOCAL_GROUPS} from '../urls'

export const allLocalGroupsGet = dispatch => apiAction(dispatch, ALL_LOCAL_GROUPS_GET, LOCAL_GROUPS)
export const localGroupGet = (dispatch, localGroup) => apiAction(dispatch, LOCAL_GROUP_GET, LOCAL_GROUPS + localGroup)