import React from "react"
import { PropTypes } from "prop-types"
import { BrowserRouter as Router } from "react-router-dom"

import { Header } from "./header"
import { Routes } from "./routes"
import { ErrorBoundary } from "./routes/error/"

export const Dodo = ({ startup }) => {
	startup()
	return (
		<ErrorBoundary>
			<Router>
				<Header />
				<Routes />
			</Router>
		</ErrorBoundary>
	)
}

Dodo.propTypes = {
	startup: PropTypes.func
}
