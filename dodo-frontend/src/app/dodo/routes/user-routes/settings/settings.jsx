import { Container, Error, TickBox } from "app/components/forms";
import {
	addReturnPathToRoute,
	WARNING__BLOCK_JS_THREAD_FOR_MS
} from "app/domain/services/services";
import { updateDetails as _updateDetails } from "app/domain/user/actions";
import {
	emailPreferences as _emailPreferences,
	fetchingUser as _fetchingUser,
	guid as _guid,
	isUpdating as _isUpdating,
	updateError as _updateError,
	username as _username
} from "app/domain/user/selectors";
import { useAction } from "app/hooks/useAction";
import { useBeforeUnload } from "app/hooks/useBeforeUnload";
import PropTypes from "prop-types";
import React, { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useHistory, useLocation } from "react-router-dom";
import { useDebouncedCallback } from "use-debounce/lib";
import { LOGIN_ROUTE } from "../login/route";
import { ChangePw } from "./change-pw/change-pw";
import { DeleteUser } from "./delete-user/delete-user";
import styles from "./settings.module.scss";

export const Settings = () => {
	const fetchingUser = useSelector(_fetchingUser);
	const isUpdating = useSelector(_isUpdating);
	const updateError = useSelector(_updateError);

	const guid = useSelector(_guid);
	const username = useSelector(_username);
	const {
		dailyUpdate = false,
		weeklyUpdate = false,
		newNotifications = false,
	} = useSelector(_emailPreferences) || {};

	const updateEmailPrefs = useAction(_updateDetails);
	const updatePrefsDebounce = useDebouncedCallback(updateEmailPrefs, 1500);

	const history = useHistory();
	const { pathname } = useLocation();
	!username &&
		!fetchingUser &&
		history.push(addReturnPathToRoute(LOGIN_ROUTE, pathname));

	const [DUToggle, setDUToggle] = useState(dailyUpdate);
	const [WUToggle, setWUToggle] = useState(weeklyUpdate);
	const [NNToggle, setNNToggle] = useState(newNotifications);

	if (!updatePrefsDebounce.pending() && !isUpdating) {
		dailyUpdate === DUToggle || setDUToggle(dailyUpdate);
		weeklyUpdate === WUToggle || setWUToggle(weeklyUpdate);
		newNotifications === NNToggle || setNNToggle(newNotifications);
	}

	/** Puts the needed preferences into an args array. Don't forget to use '...' to unpack! */
	const getUpdatePrefsParameters = useCallback(
		/**
		 * @param {boolean} keepalive
		 * @return {[string, any, boolean]}
		 */
		(additionalUpdates = {}, keepalive = false) => [
			guid,
			{
				personalData: {
					emailPreferences: {
						dailyUpdate: DUToggle,
						weeklyUpdate: WUToggle,
						newNotifications: NNToggle,
						...additionalUpdates,
					},
				},
			},
			keepalive,
		],
		[DUToggle, NNToggle, WUToggle, guid]
	);

	const updatePrefsDebounced = useCallback(
		(emailPreferences) =>
			updatePrefsDebounce.callback(
				...getUpdatePrefsParameters(emailPreferences)
			),
		[updatePrefsDebounce, getUpdatePrefsParameters]
	);

	/** Immediately sends any pending updates */
	const sendUpdateBeforePageCloses = useCallback(() => {
		if (updatePrefsDebounce.pending()) {
			updatePrefsDebounce.cancel();
			updateEmailPrefs(...getUpdatePrefsParameters({}, true));
			WARNING__BLOCK_JS_THREAD_FOR_MS(500); // freezes for just long enough to get request out
		}
	}, [getUpdatePrefsParameters, updateEmailPrefs, updatePrefsDebounce]);

	useBeforeUnload(sendUpdateBeforePageCloses); // runs if page is closing or reloading
	useEffect(() => updatePrefsDebounce.flush, [updatePrefsDebounce]); // runs when component unmounts

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
					<Error
						error={
							updateError &&
							"Oops, something went wrong. Please refresh the page and try again"
						}
					/>
					<ChangePw />
					<DeleteUser />
				</>
			}
		/>
	);
};

Settings.propTypes = {
	currentUsername: PropTypes.string,
};
