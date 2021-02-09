import React, { useEffect } from "react";
import { Switch } from "react-router";
import { Route, useHistory } from "react-router-dom";

import { NotFound } from "./error";
import { Resource, RESOURCE_ROUTE } from "./resource";
import { JOIN_RESOURCE_ROUTE } from "./resource/route";
import { Search, SEARCH_ROUTE } from "./search";
import {
	Login,
	LOGIN_ROUTE,
	MY_REBELLION_ROUTE,
	MyRebellion,
	Register,
	REGISTER_ROUTE,
	RESET_PASSWORD_ROUTE,
	ResetPassword,
	Settings,
	SETTINGS_ROUTE,
} from "./user-routes";

export const Routes = ({ closeMenu }) => {
	const history = useHistory();

	useEffect(() => {
		closeMenu();
	}, [history.location, closeMenu]);

	return (
		<Switch>
			<Route path={SEARCH_ROUTE} component={Search} exact />
			<Route path={LOGIN_ROUTE} component={Login} exact />
			<Route
				path={RESET_PASSWORD_ROUTE}
				component={ResetPassword}
				exact
			/>
			<Route path={REGISTER_ROUTE} component={Register} exact />
			<Route path={MY_REBELLION_ROUTE} component={MyRebellion} exact />
			<Route
				path={[RESOURCE_ROUTE, JOIN_RESOURCE_ROUTE]}
				component={Resource}
				exact
			/>
			<Route path={SETTINGS_ROUTE} component={Settings} exact />
			<Route component={NotFound} />
		</Switch>
	);
};
