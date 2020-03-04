import { path } from 'ramda'

export const rebellions = (state) =>
	path(['domain', 'rebellions'], state)
