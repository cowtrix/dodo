import { path } from 'ramda'
import { LOGIN, GET_LOGGED_IN_USER, REGISTER_USER, GET_MY_REBELLION, UPDATE_DETAILS, CHANGE_PASSWORD } from './action-types'

export const user = state =>
	path(["domain", "user"], state)

export const name = state =>
	path(["domain", "user", "name"], state)

export const username = state =>
	path(["domain", "user", "slug"], state)

export const email = state =>
	path(["domain", "user", "personalData", "email"], state)

export const guid = state =>
	path(["domain", "user", "guid"], state)

export const memberOf = state =>
	path(["domain", "user", "metadata", "memberOf"], state)

export const emailConfirmed = state =>
	path(["domain", "user", "personalData", "emailConfirmed"], state)

export const emailPreferences = /**
 * @return {{ dailyUpdate: boolean, weeklyUpdate: boolean, newNotifications: boolean }}
 */
 state =>
	path(["domain", "user", "personalData", "emailPreferences"], state)

export const myRebellion = state =>
	path(["domain", "user", "myRebellion"], state)

export const error = state =>
	path(["requests", LOGIN, "error"], state)

export const isLoggingIn = state =>
	path(["requests", LOGIN, "isFetching"], state)

export const fetchingMyRebellion = state =>
	path(["requests", GET_MY_REBELLION, "isFetching"], state) && !path(["requests", GET_MY_REBELLION, "hasErrored"], state)

export const fetchingMyRebellionError = state =>
	path(["requests", GET_MY_REBELLION, "error"], state)

export const fetchingUser = state =>
	path(["requests", GET_LOGGED_IN_USER, "isFetching"], state) && !path(["requests", GET_LOGGED_IN_USER, "hasErrored"], state)

export const registeringUser = state =>
	path(["requests", REGISTER_USER, "isFetching"], state) && !path(["requests", REGISTER_USER, "hasErrored"], state)

export const changingPw = state =>
	path(["requests", CHANGE_PASSWORD, "isFetching"], state)

export const changePwError = state =>
	path(["requests", CHANGE_PASSWORD, "error"], state)

export const registerError = state =>
	path(["requests", REGISTER_USER, "error"], state)

export const updateError = state =>
	path(["requests", UPDATE_DETAILS, "error"], state)

export const isUpdating = state =>
	path(["requests", UPDATE_DETAILS, "isFetching"], state)
