import React, { useState } from 'react'
import PropTypes from 'prop-types'
import { SignUpButton } from '../sign-up-button'
import { TextArea } from '../../forms'
import styles from './role.module.scss'


const APPLICANT_ANSWER_INPUT = 'APPLICANT_ANSWER_INPUT'
const APPY_NOW = 'Apply now'

export const Role = ({ applicantQuestion, isLoggedIn, applyForRole, resourceColor, hasApplied }) => {
	const [ applicantAnswer, setApplicantAnswer ] = useState('')

	return (
		isLoggedIn && applicantQuestion && !hasApplied ?
			<div className={styles.role}>
				<div>
					{applicantQuestion}
				</div>
				<TextArea
					type="text"
					id={APPLICANT_ANSWER_INPUT}
					value={applicantAnswer}
					setValue={setApplicantAnswer}
					className={styles.textArea}
				/>
				<SignUpButton
					onClick={() => applyForRole(applicantAnswer)}
					CTA={APPY_NOW}
					resourceColor={resourceColor}
					className={styles.signUpButton}
				/>
			</div> :
				hasApplied ?
					<h2>You have already applied for this role</h2> :
			null
	)
}


Role.propTypes = {
	applicantQuestion: PropTypes.string,
	hasApplied: PropTypes.bool,
	isLoggedIn: PropTypes.bool,
	applyForRole: PropTypes.func,
	resourceColor: PropTypes.string,
}