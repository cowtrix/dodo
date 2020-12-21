import { useEffect } from 'react'
import { useSelector } from 'react-redux';
import { useHistory, useRouteMatch } from 'react-router-dom';

import { username, fetchingUser } from 'app/domain/user/selectors'
import { LOGIN_ROUTE } from 'app/dodo/routes/user-routes/login/route';
import { addReturnPathToRoute } from 'app/domain/services/services';

export const useRequireLogin = () => {
	const history = useHistory();
	const { path } = useRouteMatch();

	const _username = useSelector(username);
	const _fetchingUser = useSelector(fetchingUser);

	useEffect(() => {
		if(!_username && !_fetchingUser) {
			history.replace(
				addReturnPathToRoute(LOGIN_ROUTE, path)
			);
		}
	}, [_username, _fetchingUser, history, path])

	return !!_username || !!_fetchingUser;
}

export default useRequireLogin;
