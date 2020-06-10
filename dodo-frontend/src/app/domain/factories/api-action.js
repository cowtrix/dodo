import { api } from "../services"
import { REQUEST, FAILURE, SUCCESS } from "../constants"

export const apiAction = async (dispatch, action, url, cb, abortSignal, method, body) => {
	dispatch({
		type: action + REQUEST,
		payload: {
			action,
			url
		}
	})
	return api(url, method, body, abortSignal)
		.then(response => {
			dispatch({
				type: action + SUCCESS,
				payload: response
			})
			if (cb) cb(response)
		})
		.catch(error => {
			console.log(error)
			if (cb) cb(false)
			dispatch({
				type: action + FAILURE,
				payload: error
			})
		})
}
