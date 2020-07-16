import React from "react"
import { PropTypes } from "prop-types"
import { BrowserRouter as Router } from "react-router-dom"

import { Header } from "./header"
import { Routes } from "./routes"
import { ErrorBoundary } from "./routes/error/"
import { UserMenu } from './user-menu'
import { AppLoadingScreen } from './app-loading-screen'

export const Dodo = ({ startup }) => {
	startup()
	return (
		<ErrorBoundary>
			<AppLoadingScreen/>
			<Router>
				<Header />
				<UserMenu />
				<Routes />
			</Router>
		</ErrorBoundary>
	)
}

Dodo.propTypes = {
	startup: PropTypes.func
}
