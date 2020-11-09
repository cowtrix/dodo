import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useLocation } from 'react-router-dom';

import { keepReturnPathParam } from '../../../../domain/services/services';
import { LOGIN_ROUTE } from '../../../routes/user-routes/login';
import styles from './login-register.module.scss';

export const LoginRegister = () => {
	const location = useLocation()

	const { t } = useTranslation("ui")

	return (
		<Link to={keepReturnPathParam(LOGIN_ROUTE, location)} className={styles.loginRegister}>
			{t("header_sign_in_text")}
		</Link>
	)
}

