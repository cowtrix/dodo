import React, { Fragment } from 'react'
import { PropTypes } from 'prop-types'

import { Header } from "./header"
import { Routes } from './routes'

export const Dodo = ({ startup }) => {
	startup()
	return (
		<Fragment>
			<Header/>
			<Routes/>
		</Fragment>
	)
}

Dodo.propTypes = {
	startup: PropTypes.func,
}
