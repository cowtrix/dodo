import React from 'react'
import { PropTypes } from 'prop-types'
import { BrowserRouter as Router } from 'react-router-dom'

import { Header } from "./header"
import { Routes } from './routes'

export const Dodo = ({ startup }) => {
	startup()
	return (
		<Router>
			<Header/>
			<Routes/>
		</Router>
	)
}

Dodo.propTypes = {
	startup: PropTypes.func,
}
