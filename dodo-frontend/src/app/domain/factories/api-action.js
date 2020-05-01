import { api } from "../services"
import { REQUEST, FAILURE, SUCCESS } from "../constants"

export const apiAction = async (dispatch, action, url, method, body, cb) => {
	dispatch({
		type: action + REQUEST,
		payload: {
			action,
			url
		}
	})
	return api(url, method, body)
		.then(response => {
			if (cb) cb(response.success)
			dispatch({
				type: action + SUCCESS,
				payload: response
			})
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
