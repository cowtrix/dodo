import { postLogin } from '../services'
import { LOGIN, RESET_PASSWORD, REGISTER_USER, GET_LOGGED_IN_USER, UPDATE_DETAILS } from './action-types'
import { apiAction } from '../factories'
import { RESET_PASSWORD as RESET_PASSWORD_URL, REGISTER_USER as REGISTER_USER_URL, AUTH_URL } from '../urls'


import { REQUEST, SUCCESS, FAILURE } from '../constants'

export const login = (dispatch, username, password, rememberMe) => {
	dispatch({
		type: LOGIN + REQUEST,
		payload: LOGIN
	})
	postLogin(username, password, rememberMe)
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

export const resetPassword = (dispatch, email, cb) =>
	apiAction(dispatch, RESET_PASSWORD, RESET_PASSWORD_URL + '?email=' + email, cb)

export const registerUser = (dispatch, userDetails) =>
	apiAction(dispatch, REGISTER_USER, REGISTER_USER_URL, (success) => dispatch({
	type: LOGIN + SUCCESS, payload: success
}), false, 'post', userDetails)

export const getLoggedInUser = (dispatch) =>
	apiAction(dispatch, GET_LOGGED_IN_USER, AUTH_URL)

export const updateDetails = (dispatch, guid, details) =>
	apiAction(dispatch, UPDATE_DETAILS, AUTH_URL + guid, (success) => dispatch({
		type: LOGIN + SUCCESS, payload: success
	}), false, 'PATCH', details)
