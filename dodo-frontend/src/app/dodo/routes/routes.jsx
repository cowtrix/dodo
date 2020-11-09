import React, { useEffect } from "react"
import { Route } from "react-router-dom"
import { Switch } from "react-router"

import { Search, SEARCH_ROUTE } from "./search"
import { RESOURCE_ROUTE, Resource } from "./resource"

import { NotFound } from "./error"
import {
	ResetPassword,
	RESET_PASSWORD_ROUTE,
	Register,
	REGISTER_ROUTE,
	YourProfile,
	YOUR_PROFILE_ROUTE,
	MyRebellion,
	MY_REBELLION_ROUTE,
	Login,
	LOGIN_ROUTE
} from './user-routes'


import { useHistory } from 'react-router-dom'
import { JOIN_RESOURCE_ROUTE } from "./resource/route"

export const Routes = ({ closeMenu }) => {
	const history = useHistory()

	useEffect(() => {
		closeMenu()
	}, [history.location, closeMenu])

	return (
		<Switch>
			<Route path={SEARCH_ROUTE} component={Search} exact/>
			<Route path={LOGIN_ROUTE} component={Login} exact/>
			<Route path={RESET_PASSWORD_ROUTE} component={ResetPassword} exact/>
			<Route path={REGISTER_ROUTE} component={Register} exact/>
			<Route path={YOUR_PROFILE_ROUTE} component={YourProfile} exact/>
			<Route path={MY_REBELLION_ROUTE} component={MyRebellion} exact/>
			<Route path={[RESOURCE_ROUTE, JOIN_RESOURCE_ROUTE]} component={Resource} exact/>
			<Route component={NotFound}/>
		</Switch>
	)
}
