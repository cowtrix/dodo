import { path } from 'ramda'
import { LOGIN } from './action-types'
import { FAILURE } from '../constants'

export const username = state =>
	path(["domain", "user", "name"], state)

export const error = state =>
	path(["requests", LOGIN, "error"], state)