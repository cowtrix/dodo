import React from 'react'
import { Route, BrowserRouter as Router } from 'react-router-dom'
import { Home, route as home } from './home'

export const Routes = () =>
	<Router>
		<Route path={home} component={Home} />
	</Router>
