import { api, auth } from "../services"
import { CANCELLED, REQUEST, FAILURE, SUCCESS } from "../constants"

const abortControllers = {};

function abortPreviousRequest(requestKey, dispatch, action, cb) {
	if(abortControllers[requestKey]) {
		abortControllers[requestKey].abort(); // Abort last request if there is one
		delete abortControllers[requestKey];
		if (cb) cb(false); // Call callback function if one exists

		// Dispatch cancelled action
		dispatch({
			type: action + CANCELLED,
			payload: {
				status: 0,
				message: "The user aborted the request."
			}
		})
	}
}

const apiActionHandler = async (service, dispatch, action, url, cb, abortPrevious, method, body, keepalive = false) => {
	let abortController;
	const requestKey = method + ':' + url.split(/[?#]/)[0]; // Get method:URL (path without params or hashes) to use as request key

	if(abortPrevious) {
		abortController = new AbortController();

		// Cancel previous request if abortPrevious flag is set
		if(abortPrevious) abortPreviousRequest(requestKey, dispatch, action, cb);

		// Store abortController so we can cancel request later if we need to
		abortControllers[requestKey] = abortController;
	}

	dispatch({
		type: action + REQUEST,
		payload: {
			action,
			url
		}
	})

	return service(url, method, body, abortController, keepalive)
		.then(response => {
			if (response.status) {
				dispatch({
					type: action + FAILURE,
					payload: response
				})
			}
			else {
				dispatch({
					type: action + SUCCESS,
					payload: response
				})
			}
			delete abortControllers[requestKey];
			if (cb) cb(response)
		})
		.catch(error => {
			if(error.status !== 0) { // status of 0 is cancelled, anything above that is a failure, less than is network error
				console.log(error)
				delete abortControllers[requestKey];
				if (cb) cb(false)
				dispatch({
					type: action + FAILURE,
					payload: error
				})
			}
		})
}

export const apiAction = (dispatch, action, url, cb, abortPrevious, method, body, keepalive = false) => {
	return apiActionHandler(api, dispatch, action, url, cb, abortPrevious, method, body, keepalive);
}

export const authAction = (dispatch, action, url, cb, abortPrevious, method, body, keepalive = false) => {
	return apiActionHandler(auth, dispatch, action, url, cb, abortPrevious, method, body, keepalive);
}
