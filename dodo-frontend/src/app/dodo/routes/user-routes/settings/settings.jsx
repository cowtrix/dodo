import { Container, Error, Submit, TickBox } from "app/components/forms";
import { addReturnPathToRoute } from "app/domain/services/services";
import {
	resendVerificationEmail as _resendVerificationEmail,
	updateDetails as _updateDetails,
} from "app/domain/user/actions";
import {
	email as _email,
	emailConfirmed as _emailConfirmed,
	emailPreferences as _emailPreferences,
	fetchingUser as _fetchingUser,
	guid as _guid,
	isUpdating as _isUpdating,
	updateError as _updateError,
	username as _username,
} from "app/domain/user/selectors";
import { useAction } from "app/hooks/useAction";
import { useBeforeUnload } from "app/hooks/useBeforeUnload";
import PropTypes from "prop-types";
import React, { useCallback, useState } from "react";
import { useSelector } from "react-redux";
import { Prompt, useHistory, useLocation } from "react-router-dom";

import { LOGIN_ROUTE } from "../login/route";
import { UpdateEmail } from "./update-email";
import { ChangePw } from "./change-pw/change-pw";
import { DeleteUser } from "./delete-user/delete-user";
import styles from "./settings.module.scss";

export const Settings = () => {
	const fetchingUser = useSelector(_fetchingUser);
	const isUpdating = useSelector(_isUpdating);
	const updateError = useSelector(_updateError);
	const emailConfirmed = useSelector(_emailConfirmed);
	const currentEmail = useSelector(_email);

	const guid = useSelector(_guid);
	const username = useSelector(_username);
	const { dailyUpdate, weeklyUpdate, newNotifications } =
		useSelector(_emailPreferences) || {};

	const updateEmailPrefs = useAction(_updateDetails);
	const resendVerificationEmail = useAction(_resendVerificationEmail);

	const history = useHistory();
	const { pathname } = useLocation();

	if (!username && !fetchingUser) {
		history.push(addReturnPathToRoute(LOGIN_ROUTE, pathname));
	}

	const [initialDataFilled, setInitialDataFilled] = useState(false);
	const [unsavedChanges, setUnsavedChanges] = useState(false);

	const [DUToggle, setDUToggle] = useState(dailyUpdate);
	const [WUToggle, setWUToggle] = useState(weeklyUpdate);
	const [NNToggle, setNNToggle] = useState(newNotifications);
	const [emailEdit, setEmailEdit] = useState(currentEmail);

	if (!initialDataFilled && !isUpdating && !fetchingUser) {
		if (dailyUpdate !== DUToggle) setDUToggle(dailyUpdate);
		if (weeklyUpdate !== WUToggle) setWUToggle(weeklyUpdate);
		if (newNotifications !== NNToggle) setNNToggle(newNotifications);
		if (currentEmail !== emailEdit) setEmailEdit(currentEmail);
		setInitialDataFilled(true);
	}

	const handleUpdate = () => {
		updateEmailPrefs(guid, {
			personalData: {
				email: emailEdit,
				emailPreferences: {
					dailyUpdate: dailyUpdate,
					weeklyUpdate: weeklyUpdate,
					newNotifications: newNotifications,
				},
			},
		});
		setUnsavedChanges(false);
	};

	const handleUnload = useCallback(
		(event) => {
			// methods (and fallbacks for support) to ask the user
			// if they're sure they want to close the page
			if (unsavedChanges) {
				event.preventDefault();
				event.returnValue = "";
				return "";
			}
		},
		[unsavedChanges]
	);
	useBeforeUnload(handleUnload);

	const handleSet = (setter) => (...args) => {
		setUnsavedChanges(true);
		setter(...args);
	};

	return (
		<>
			<Container
				loading={fetchingUser}
				title="Settings"
				content={
					<>
						<Prompt
							when={unsavedChanges}
							message={() =>
								`You have unsaved changes - are you sure you want to leave this page?`
							}
						/>
						<Container
							loading={isUpdating}
							isSubContainer
							content={
								<>
									<div>
										<h3 className={styles.h3Title}>
											Email Preferences
										</h3>
										<div className={styles.text}>
											Here you can adjust what emails you
											receive from us.
										</div>
									</div>
									{/* <TickBox
										name="Daily Update"
										id="dailyUpdate"
										checked={DUToggle ?? false}
										setValue={handleSet(setDUToggle)}
									/>
									<TickBox
										name="Weekly Update"
										id="weeklyUpdate"
										checked={WUToggle ?? false}
										setValue={handleSet(setWUToggle)}
									/> */}
									<TickBox
										name="Notifications"
										id="newNotifications"
										checked={NNToggle ?? false}
										setValue={handleSet(setNNToggle)}
									/>
									<UpdateEmail
										email={emailEdit}
										setEmail={handleSet(setEmailEdit)}
										isConfirmed={emailConfirmed}
										resendVerificationEmail={
											resendVerificationEmail
										}
									/>
									<Error
										error={
											updateError &&
											"Oops, something went wrong - looks like a network error. Please refresh the page and try again"
										}
									/>
									<Submit
										value="Update my details"
										className={styles.updateButton}
										submit={handleUpdate}
									/>
								</>
							}
						/>
						<ChangePw />
						<DeleteUser />
					</>
				}
			/>
		</>
	);
};

Settings.propTypes = {
	currentUsername: PropTypes.string,
};
