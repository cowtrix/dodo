import { postLogin } from '../services'
import { LOGIN } from './action-types'
import { REQUEST, SUCCESS, FAILURE } from '../constants'

export const login = (dispatch, username, password) => {
	dispatch({
		type: LOGIN + REQUEST,
		payload: LOGIN
	})
	postLogin(username, password)
		.then(response => {
			if (response.status === 404) {
				dispatch({
					type: LOGIN + FAILURE,
					payload: "Unknown username or password"
				})
			} else {
				dispatch({
					type: LOGIN + SUCCESS,
					payload: response
				})
			}
		})
		.catch(error => {
			dispatch({
				type: LOGIN + FAILURE,
				payload: error
			})
		})
}
