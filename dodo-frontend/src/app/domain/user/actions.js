import {
	LOGIN,
	RESET_PASSWORD,
	REGISTER_USER,
	GET_LOGGED_IN_USER,
	GET_MY_REBELLION,
	UPDATE_DETAILS,
	LOGOUT,
	RESEND_VALIDATION_EMAIL,
	CHANGE_PASSWORD,
	DELETE_USER
} from './action-types'
import { apiAction, authAction } from '../factories'

import {
	RESET_PASSWORD as RESET_PASSWORD_URL,
	CHANGE_PASSWORD as CHANGE_PASSWORD_URL,
	REGISTER_USER as REGISTER_USER_URL,
	AUTH_URL,
	LOGIN as LOGIN_URL,
	LOGOUT_URL,
	MY_REBELLION_URL,
	RESEND_VALIDATION_EMAIL_URL
} from '../urls'

import { SUCCESS } from '../constants'

export const login = (dispatch, username, password, rememberMe) => {
	const body = {
		username: username,
		password: password,
		rememberMe: rememberMe,
		redirect: ""
	};
	return apiAction(dispatch, LOGIN, LOGIN_URL, undefined, undefined, 'post', body);
}

export const resetPassword = (dispatch, email, cb) =>
	apiAction(dispatch, RESET_PASSWORD, RESET_PASSWORD_URL + '?email=' + email, cb)

export const changePassword = (dispatch, currentpassword, newpassword, cb) =>
	apiAction(dispatch, CHANGE_PASSWORD, CHANGE_PASSWORD_URL, cb, undefined, "post", {
		currentpassword,
		newpassword,
	});

export const registerUser = (dispatch, userDetails) =>
	apiAction(dispatch, REGISTER_USER, REGISTER_USER_URL, (success) => dispatch({
	type: LOGIN + SUCCESS, payload: success
}), false, 'post', userDetails)

export const deleteUser = (dispatch, guid, cb) => 
	authAction(dispatch, DELETE_USER, AUTH_URL + guid, cb, undefined, 'delete')

export const getLoggedInUser = (dispatch) =>
	authAction(dispatch, GET_LOGGED_IN_USER, AUTH_URL)

export const getMyRebellion = (dispatch) =>
	authAction(dispatch, GET_MY_REBELLION, MY_REBELLION_URL)

export const updateDetails = (dispatch, guid, details) =>
	authAction(dispatch, UPDATE_DETAILS, AUTH_URL + guid, (success) => dispatch({
		type: LOGIN + SUCCESS, payload: success
	}), false, 'PATCH', details)

export const logUserOut = (dispatch, customCb = undefined) =>
	apiAction(dispatch, LOGOUT, LOGOUT_URL, () => {
		customCb && customCb();
		refreshPage();
	})

export const resendVerificationEmail = (dispatch) =>
	apiAction(dispatch, RESEND_VALIDATION_EMAIL, RESEND_VALIDATION_EMAIL_URL)

const refreshPage = () => {
	window.location.reload()
	return false
}
