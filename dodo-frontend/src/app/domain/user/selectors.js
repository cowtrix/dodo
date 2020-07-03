import { path } from 'ramda'
import { LOGIN, GET_LOGGED_IN_USER } from './action-types'

export const user = state =>
	path(["domain", "user"], state)

export const name = state =>
	path(["domain", "user", "name"], state)

export const username = state =>
	path(["domain", "user", "slug"], state)

export const email = state =>
	path(["domain", "user", "personalData", "email"], state)

export const error = state =>
	path(["requests", LOGIN, "error"], state)

export const fetchingUser = state =>
	path(["requests", GET_LOGGED_IN_USER, "isFetching"], state)