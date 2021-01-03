import { Container, Input, Submit } from "app/components/forms";
import {
	passwordContainsNoSymbol,
	strNotEmptyAndLengthBelow
} from "app/domain/services/services";
import {
	changePassword as _changePassword,
	logUserOut as _logUserOut
} from "app/domain/user/actions";
import {
	changePwError as _changePwError,
	changingPw as _changingPw
} from "app/domain/user/selectors";
import { useAction } from "app/hooks/useAction";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import styles from "./change-pw.module.scss";

export const ChangePw = () => {
	const changingPw = useSelector(_changingPw);
	const changePwErrorResponse = useSelector(_changePwError);
	const changePwErrorUnauth = changePwErrorResponse?.message?.includes?.(
		"Unauthorized"
	);

	const changePassword = useAction(_changePassword);
	const logUserOut = useAction(_logUserOut);

	const [currentPw, setCurrentPw] = useState("");
	const [newPw, setNewPw] = useState("");
	const [confirmNewPw, setConfirmNewPw] = useState("");

	const newPwIsSameAsCurr = newPw.length && newPw === currentPw;
	const newPwIsInvalid = !!(
		strNotEmptyAndLengthBelow(8, newPw) ||
		(newPw.length && passwordContainsNoSymbol(newPw)) ||
		newPwIsSameAsCurr
	);

	const confirmNewPwIsInvalid =
		newPw !== confirmNewPw && !!confirmNewPw.length;

	const formInvalid =
		!currentPw.length ||
		!newPw.length ||
		!confirmNewPw.length ||
		newPwIsInvalid ||
		confirmNewPwIsInvalid;

	const clearForm = () => {
		setCurrentPw("");
		setNewPw("");
		setConfirmNewPw("");
	};

	return (
		<div className={styles.warning}>
			<Container
				loading={changingPw}
				content={
					<form
						onSubmit={(e) => {
							// stops page from instantly reloading on submit
							e.preventDefault();

							!formInvalid &&
								changePassword(currentPw, confirmNewPw, (v) => {
									v === false ? clearForm() : logUserOut();
								});
						}}
					>
						<h3 className={styles.h3Title}>Change your password</h3>
						<p className={styles.paragraph}>
							Note: A successful password change will mean you
							need to log in again.
						</p>
						<Input
							id="current-pw"
							type="password"
							placeholder="Current password..."
							value={currentPw}
							setValue={setCurrentPw}
							maxLength={63}
							error={
								!!changePwErrorResponse &&
								currentPw.length === 0
							}
							errorJustOnMsg={!!changePwErrorResponse}
							message={
								(changePwErrorUnauth &&
									"Current password was entered incorrectly. Please try again") ||
								(changePwErrorResponse &&
									"Oops, something went wrong. Please try again") ||
								undefined
							}
						/>
						<Input
							id="new-pw"
							type="password"
							placeholder="New password..."
							value={newPw}
							setValue={setNewPw}
							maxLength={63}
							error={newPwIsInvalid}
							message={
								(newPwIsSameAsCurr &&
									"You must choose a different password to your current one") ||
								(newPwIsInvalid &&
									"Password must be 8 characters or longer, and contain a symbol") ||
								undefined
							}
						/>
						<Input
							id="confirm-new-pw"
							type="password"
							placeholder="Confirm password..."
							value={confirmNewPw}
							setValue={setConfirmNewPw}
							maxLength={63}
							error={confirmNewPwIsInvalid}
							message={
								(confirmNewPwIsInvalid &&
									"New passwords must match") ||
								undefined
							}
						/>
						<div className={styles.greyBackground}>
							<Submit
								value="Change Password"
								disabled={formInvalid}
							/>
						</div>
					</form>
				}
			/>
		</div>
	);
};
