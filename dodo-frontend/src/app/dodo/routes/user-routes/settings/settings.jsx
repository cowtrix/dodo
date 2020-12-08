import React, { useState, useEffect } from 'react'
import PropTypes from 'prop-types'
import { Container, Input, Submit, TickBox } from 'app/components/forms'
import { addReturnPathToRoute } from 'app/domain/services/services'
import { useHistory, useLocation } from 'react-router-dom';
import { LOGIN_ROUTE } from '../login/route';
import styles from './settings.module.scss';
import { Button } from '../../../../components/button';

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
					<div>
						<h3 className={styles.h3Title}>Email Preferences</h3>
						<div className={styles.text}>
							Here you can adjust what emails you receive
							from us.
						</div>
					</div>
					<TickBox
						name="Daily Update"
						id="dailyUpdate"
					/>
					<TickBox
						name="Weekly Update"
						id="weeklyUpdate"
					/>
					<TickBox
						name="Notifications"
						id="notifications"
					/>
					<div className={styles.warning}>
						<h3 className={styles.h3Title}>Change your password</h3>
						<Input
							id="current-pw"
							type="password"
							placeholder="Current password..."
							value={currentPw}
							setValue={setCurrentPw}
						/>
						<Input
							id="new-pw"
							type="password"
							placeholder="New password..."
							value={newPw}
							setValue={setNewPw}
						/>
						<Input
							id="confirm-new-pw"
							type="password"
							placeholder="Confirm password..."
							value={confirmNewPw}
							setValue={setConfirmNewPw}
						/>
						<Submit
							value="Change Password"
							submit={()=>{}}
						/>
					</div>

					<div className={styles.dangerZoneBox}>
						<div className={styles.dangerZoneTitle}>
							<h2>Danger Zone</h2>
						</div>
						<div className={styles.dangerZoneInner}>
							<div>
								This will permanently and irreversibly delete
								your account and all activity associated with it.
								This action cannot be undone.
							</div>
							<Button variant="cta-danger">
								<div>Delete Your Account</div>
							</Button>
						</div>
					</div>
				</>
			}
		/>
	)
}


Settings.propTypes = {
	currentUsername: PropTypes.string
}