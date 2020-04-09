import { SEARCH_FILTER_EVENTS, SEARCH_GET } from "./action-types";
import { apiAction } from "../factories/api-action";
import { SEARCH } from "../urls";

import { addParamsToUrl } from "../services";

export const searchGet = (dispatch, params) =>
	apiAction(dispatch, SEARCH_GET, addParamsToUrl(SEARCH, params));

export const searchFilterEvents = payload => ({
	type: SEARCH_FILTER_EVENTS,
	payload
});
