import { Button, ExpandPanel } from "app/components";
import { Container, TickBox } from "app/components/forms";
import { addReturnPathToRoute } from "app/domain/services/services";
import { updateDetails as _updateDetails } from "app/domain/user/actions";
import {
	emailPreferences as _emailPreferences,
	fetchingUser as _fetchingUser,
	guid as _guid,
	isUpdating as _isUpdating,
	username as _username,
} from "app/domain/user/selectors";
import { useAction } from "app/hooks/useAction";
import PropTypes from "prop-types";
import React, { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useHistory, useLocation } from "react-router-dom";
import { useDebouncedCallback } from "use-debounce/lib";

import { LOGIN_ROUTE } from "../login/route";
import { ChangePw } from "./change-pw/change-pw";
import styles from "./settings.module.scss";

export const Settings = () => {
	const fetchingUser = useSelector(_fetchingUser);
	const isUpdating = useSelector(_isUpdating);

	const guid = useSelector(_guid);
	const username = useSelector(_username);
	const {
		dailyUpdate = false,
		weeklyUpdate = false,
		newNotifications = false,
	} = useSelector(_emailPreferences) || {};

	const history = useHistory();
	const { pathname } = useLocation();

	if (!username && !fetchingUser) {
		history.push(addReturnPathToRoute(LOGIN_ROUTE, pathname));
	}

	const [DUToggle, setDUToggle] = useState(dailyUpdate);
	const [WUToggle, setWUToggle] = useState(weeklyUpdate);
	const [NNToggle, setNNToggle] = useState(newNotifications);

	const updateEmailPrefs = useAction(_updateDetails);
	const updatePrefsDebounce = useDebouncedCallback(updateEmailPrefs, 500);
	const updatePrefsDebounced = useCallback(
		(emailPreferences) => {
			return updatePrefsDebounce.callback(guid, {
				personalData: {
					emailPreferences: {
						dailyUpdate: DUToggle,
						weeklyUpdate: WUToggle,
						newNotifications: NNToggle,
						...emailPreferences,
					},
				},
			});
		},
		[updatePrefsDebounce, guid, DUToggle, WUToggle, NNToggle]
	);

	useEffect(() => {
		if (!updatePrefsDebounce.pending() && !isUpdating) {
			dailyUpdate === DUToggle || setDUToggle(dailyUpdate);
			weeklyUpdate === WUToggle || setWUToggle(weeklyUpdate);
			newNotifications === NNToggle || setNNToggle(newNotifications);
		}
	}, [
		DUToggle,
		NNToggle,
		WUToggle,
		dailyUpdate,
		isUpdating,
		newNotifications,
		updatePrefsDebounce,
		weeklyUpdate,
	]);

	// runs when component unmounts, immediately sends any debounced updates
	useEffect(() => () => updatePrefsDebounce.flush(), [updatePrefsDebounce]);

	return (
		<Container
			loading={fetchingUser}
			title="Settings"
			content={
				<>
					<div>
						<h3 className={styles.h3Title}>Email Preferences</h3>
						<div className={styles.text}>
							Here you can adjust what emails you receive from us.
						</div>
					</div>
					<TickBox
						name="Daily Update"
						id="dailyUpdate"
						checked={DUToggle}
						setValue={(v) => {
							updatePrefsDebounced({ dailyUpdate: v });
							setDUToggle(v);
						}}
					/>
					<TickBox
						name="Weekly Update"
						id="weeklyUpdate"
						checked={WUToggle}
						setValue={(v) => {
							updatePrefsDebounced({ weeklyUpdate: v });
							setWUToggle(v);
						}}
					/>
					<TickBox
						name="Notifications"
						id="newNotifications"
						checked={NNToggle}
						setValue={(v) => {
							updatePrefsDebounced({ newNotifications: v });
							setNNToggle(v);
						}}
					/>

					<ChangePw />

					<ExpandPanel
						header={<h2>Danger Zone</h2>}
						headerClassName={styles.dangerZoneTitle}
					>
						<div className={styles.dangerZoneInner}>
							<div>
								This will permanently and irreversibly delete
								your account and all activity associated with
								it. This action cannot be undone.
							</div>
							<Button variant="cta-danger">
								<div>Delete Your Account</div>
							</Button>
						</div>
					</ExpandPanel>
				</>
			}
		/>
	);
};

Settings.propTypes = {
	currentUsername: PropTypes.string,
};
