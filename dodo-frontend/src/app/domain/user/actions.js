import {
	LOGIN,
	RESET_PASSWORD,
	REGISTER_USER,
	GET_LOGGED_IN_USER,
	UPDATE_DETAILS,
	LOGOUT,
	RESEND_VALIDATION_EMAIL
} from './action-types'
import { apiAction, authAction, loginAction } from '../factories'

import {
	RESET_PASSWORD as RESET_PASSWORD_URL,
	REGISTER_USER as REGISTER_USER_URL,
	AUTH_URL,
	LOGOUT_URL,
	RESEND_VALIDATION_EMAIL_URL
} from '../urls'

import { SUCCESS } from '../constants'

export const login = (dispatch, username, password, rememberMe) => {
	loginAction(dispatch, username, password, rememberMe);
}

export const resetPassword = (dispatch, email, cb) =>
	apiAction(dispatch, RESET_PASSWORD, RESET_PASSWORD_URL + '?email=' + email, cb)

export const registerUser = (dispatch, userDetails) =>
	apiAction(dispatch, REGISTER_USER, REGISTER_USER_URL, (success) => dispatch({
	type: LOGIN + SUCCESS, payload: success
}), false, 'post', userDetails)

export const getLoggedInUser = (dispatch) =>
	authAction(dispatch, GET_LOGGED_IN_USER, AUTH_URL)

export const updateDetails = (dispatch, guid, details) =>
	authAction(dispatch, UPDATE_DETAILS, AUTH_URL + guid, (success) => dispatch({
		type: LOGIN + SUCCESS, payload: success
	}), false, 'PATCH', details)

export const logUserOut = (dispatch) =>
	apiAction(dispatch, LOGOUT, LOGOUT_URL, refreshPage)

export const resendVerificationEmail = (dispatch) =>
	apiAction(dispatch, RESEND_VALIDATION_EMAIL, RESEND_VALIDATION_EMAIL_URL)

const refreshPage = () => {
	window.location.reload()
	return false
}
