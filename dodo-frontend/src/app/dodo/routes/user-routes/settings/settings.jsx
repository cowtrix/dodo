import React, { useState, useEffect } from 'react'
import PropTypes from 'prop-types'
import { Container, Input, Submit } from 'app/components/forms'
import { addReturnPathToRoute } from 'app/domain/services/services'
import { useHistory, useLocation } from 'react-router-dom';
import { LOGIN_ROUTE } from '../login/route';
import styles from './settings.module.scss';

export const Settings = (
	{
		currentUsername, currentName, currentEmail, fetchingUser, updateDetails, isConfirmed, guid, resendVerificationEmail }
	) => {

	const history = useHistory();
	const { pathname } = useLocation();

	if(!currentUsername && !fetchingUser) {
		history.push(addReturnPathToRoute(LOGIN_ROUTE, pathname));
	}

	const [currentPw, setCurrentPw] = useState('');
	const [newPw, setNewPw] = useState('');
	const [confirmNewPw, setConfirmNewPw] = useState('');

	useEffect(() => {

	}, [currentUsername, currentEmail, currentName])

	return (
		<Container
			loading={fetchingUser}
			title="Settings"
			content={
				<>
					<h3 className={styles.h3Title}>Change your password</h3>
					<Input
						id="current-pw"
						type="text"
						placeholder="Current password..."
						value={currentPw}
						setValue={setCurrentPw}
					/>
					<Input
						id="new-pw"
						type="text"
						placeholder="New password..."
						value={newPw}
						setValue={setNewPw}
					/>
					<Input
						id="confirm-new-pw"
						type="text"
						placeholder="Confirm password..."
						value={confirmNewPw}
						setValue={setConfirmNewPw}
					/>
					<Submit
						value="Change Password"
						submit={()=>{}}
					/>
				</>
			}
		/>
	)
}


Settings.propTypes = {
	currentUsername: PropTypes.string
}